using System.Diagnostics;
using Certstream;
using Newtonsoft.Json;
using SentinelLib;
using SentinelLib.Models;

void ResponseCallback(ScannerOutput param) {
    var json = JsonConvert.SerializeObject(
        param, 
        Formatting.Indented, 
        new JsonSerializerSettings { 
        NullValueHandling = NullValueHandling.Ignore
    });
    File.WriteAllText($"{param.InputParams?.Domain}.json", json);
    // elasticClient.Index(param, null);
}

Thread.Sleep(100);
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
            lock (client) {
                Console.Write($"{domain,-100}" +
                              $"{numdomains,-20}" +
                              $"{Math.Round(numdomains / (stopwatch.ElapsedMilliseconds / 1000f)),-10}" +
                              $"{DateTime.Now,-15}\n");
            }
            // get label
            var split = domain.Split('.');
            if (split.Length <= 2) continue;
            var label = split[0];

            // Get servicetype
            var serviceType = Helpers.StringToServiceType(label);
            if (serviceType == ServiceType.None) continue;
            switch (serviceType) {
                case ServiceType.Radarr:
                    sentinel.AddWork(new HttpScannerParams(domain, serviceType, new List<int> { 7878 }));
                    break;
                case ServiceType.Sonarr:
                    sentinel.AddWork(new HttpScannerParams(domain, serviceType, new List<int> { 8989 }));
                    break;
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