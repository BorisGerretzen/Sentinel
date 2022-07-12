using Certstream;
using MongoDB.Driver;
using Newtonsoft.Json;
using SentinelLib.Models;

MongoClient storageClient = new("mongodb://127.0.0.1:27017/?directConnection=true");
IMongoDatabase? storageDb = storageClient.GetDatabase("Scans");
var storageCollection = storageDb.GetCollection<ScannerOutput>("ScannerOutput");

async Task ResponseCallback(ScannerOutput param) {
    var json = JsonConvert.SerializeObject(
        param,
        Formatting.Indented,
        new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        });
    try {
        await storageCollection.InsertOneAsync(param);
    }
    catch (Exception e) {
        Console.WriteLine(e.Message);
    }

    File.WriteAllText($"{param.InputParams?.Domain}.json", json);
    // elasticClient.Index(param, null);
}

var workLock = new object();
var doWork = true;

Thread certThread = new(() => {
    SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, ResponseCallback);
    CertstreamClient client = new(-1);
    client.CertificateIssued += (_, cert) => {
        lock (workLock) {
            if (!doWork) client.Stop();
        }

        foreach (var domain in cert.AllDomains) {
            if (string.IsNullOrEmpty(domain)) continue;

            // get label
            var split = domain.Split('.');
            if (split.Length <= 2) continue;
            var label = split[0];

            // Get ServiceType
            ServiceType serviceType = Helpers.StringToServiceType(label);
            if (serviceType == ServiceType.None) continue;

            switch (serviceType) {
                case ServiceType.ElasticSearch:
                    sentinel.AddWork(new HttpScannerParams(domain, serviceType, new List<int> { 9200 }));
                    break;
                case ServiceType.MongoExpress:
                    sentinel.AddWork(new HttpScannerParams(domain, serviceType));
                    break;
                default:
                    sentinel.AddWork(new ScannerParams(domain, serviceType));
                    break;
            }
        }
    };
    Console.WriteLine("Sentinel has been initialized");
    client.Start();
});

Thread localHostThread = new(() => {
    SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, ResponseCallback);
    sentinel.AddWork(new ScannerParams("127.0.0.1", ServiceType.Ftp));
});
certThread.Start();
Console.ReadLine();
lock (workLock) {
    doWork = false;
}