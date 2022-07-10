using System.Diagnostics;
using Certstream;
using MongoDB.Driver;
using Newtonsoft.Json;
using SentinelLib.Models;

var storageClient = new MongoClient("mongodb://127.0.0.1:27017/?directConnection=true");
var storageDb = storageClient.GetDatabase("Scans");
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

object workLock = new object();
bool doWork = true;

Thread certThread = new Thread(() => {
    SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, ResponseCallback);
    var client = new CertstreamClient(-1);
    ulong numdomains = 0;
    Stopwatch stopwatch = new();
    client.CertificateIssued += (_, cert) => {
        lock (workLock) {
            if (!doWork) client.Stop();
        }

        foreach (var domain in cert.AllDomains) {
            if (string.IsNullOrEmpty(domain)) continue;
            numdomains++;

            // get label
            var split = domain.Split('.');
            if (split.Length <= 2) continue;
            var label = split[0];

            // Get servicetype
            var serviceType = Helpers.StringToServiceType(label);
            if (serviceType == ServiceType.None) continue;
            // lock (client) {
            //     Console.Write($"{domain,-100}" +
            //                   $"{numdomains,-20}" +
            //                   $"{Math.Round(numdomains / (stopwatch.ElapsedMilliseconds / 1000f)),-10}" +
            //                   $"{DateTime.Now,-15}\n");
            // }
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
    client.Start();
    stopwatch.Start();
});

Thread localHostThread = new Thread(() => {
    SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, ResponseCallback);
    sentinel.AddWork(new ScannerParams("127.0.0.1", ServiceType.Mongo));
});
certThread.Start();
Console.ReadLine();
lock (workLock) {
    doWork = false;
}