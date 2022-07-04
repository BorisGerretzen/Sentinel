using System.Diagnostics;
using System.Text.Json.Nodes;
using Certstream;
using Newtonsoft.Json;
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

void ResponseCallback(ScannerOutput param) {
    string json = JsonConvert.SerializeObject(param, Formatting.Indented);
    File.WriteAllText($"{param.InputParams.Domain}.json", json);
}
void WriteProgress(string s, int x) {
    int origRow = Console.CursorTop;
    int origCol = Console.CursorLeft;
    // Console.WindowWidth = 10;  // this works. 
    int width = Console.WindowWidth;
    x = x % width;
    try {
        Console.SetCursorPosition(x, 1);
        Console.Write(s);
    } catch (ArgumentOutOfRangeException e) {

    } finally {
        try {
            Console.SetCursorPosition(origCol, origRow);
        } catch (ArgumentOutOfRangeException e) {
        }
    }
}

using SentinelLib.Sentinel sentinel = new(ScannerProvider.DefaultProvider, 2, ResponseCallback);
sentinel.Start();
Thread.Sleep(100);
CertstreamClient client = new CertstreamClient(-1);
int numdomains = 0;
Stopwatch stopwatch = new();
client.CertificateIssued += (_, cert) => {
    foreach (var domain in cert.AllDomains) {
        if (string.IsNullOrEmpty(domain)) continue;
        lock (client) {
            WriteProgress(string.Concat(Enumerable.Repeat(" ", 200)), 0);
            WriteProgress(domain, 1);
            WriteProgress(numdomains.ToString(), 100);
            WriteProgress(Math.Round(numdomains / (stopwatch.ElapsedMilliseconds/1000f)).ToString(), 150);
        }

        numdomains++;

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
stopwatch.Start();
Console.ReadKey();
client.Stop();