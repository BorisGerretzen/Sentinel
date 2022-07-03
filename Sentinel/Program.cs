using Certstream;
using SentinelLib;

ServiceType StringToServiceType(string label) {
    label = label.ToLower();
    switch (label) {
        case "mongo":
            return ServiceType.Mongo;
        default:
            return ServiceType.None;
    }
}

using SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, 5);
sentinel.Start();
CertstreamClient client = new();
client.CertificateIssued += (_, cert) => {
    foreach (var domain in cert.AllDomains) {
        if (string.IsNullOrEmpty(domain)) continue;
        Console.WriteLine(domain);

        // get label
        var split = domain.Split('.');
        if (split.Length <= 2) continue;
        var label = split[0];

        // Get servicetype
        var serviceType = StringToServiceType(label);
        if (serviceType == ServiceType.None) continue;

        sentinel.AddWork(new ScannerParams(domain, serviceType));
    }
};

client.Start();
Console.ReadKey();
client.Stop();