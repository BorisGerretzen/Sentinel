using System.Diagnostics;
using Certstream;
using Newtonsoft.Json;
using SentinelLib;
using SentinelLib.Models;

ServiceType StringToServiceType(string label) {
    label = label.ToLower();
    switch (label) {
        case "mongo":
            return ServiceType.Mongo;
        case "mongoexpress":
        case "mongo-express":
            return ServiceType.MongoExpress;
        case "elastic":
        case "elasticsearch":
            return ServiceType.ElasticSearch;
        default:
            return ServiceType.None;
    }
}

void ResponseCallback(ScannerOutput param) {
    var json = JsonConvert.SerializeObject(param, Formatting.Indented);
    File.WriteAllText($"{param.InputParams?.Domain}.json", json);
    // elasticClient.Index(param, null);
}


SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, ResponseCallback);
Thread.Sleep(100);
var client = new CertstreamClient(-1);
ulong numdomains = 0;
Stopwatch stopwatch = new();

client.CertificateIssued += (_, cert) => {
    foreach (var domain in cert.AllDomains) {
        if (string.IsNullOrEmpty(domain)) continue;
        lock (client) {
            Console.Write($"{domain,-150}" +
                          $"{numdomains,10}" +
                          $"{Math.Round(numdomains / (stopwatch.ElapsedMilliseconds / 1000f)),10}\n");
        }

        numdomains++;

        // get label
        var split = domain.Split('.');
        if (split.Length <= 2) continue;
        var label = split[0];

        // Get servicetype
        var serviceType = StringToServiceType(label);
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
client.Start();
stopwatch.Start();
Console.ReadKey();
client.Stop();