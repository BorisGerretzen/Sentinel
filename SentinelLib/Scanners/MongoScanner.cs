using System.Net.Sockets;
using MongoDB.Driver;

namespace SentinelLib.Scanners; 

public class MongoScanner : Scanner{
    public MongoScanner(ScannerParams scannerParams) : base(scannerParams) { }
    protected override List<int> Ports => new() { 27017 };
    public override Dictionary<int, string?> Scan() {
        var openPorts = ScanPorts().Where(kvp => kvp.Value).Select(kvp => kvp.Key);
        Dictionary<int, string?> returnDict = new();
        foreach (var port in openPorts) {
            try {
                var client = new MongoClient($"mongodb://{_scannerParams.Domain}:{port}");
                var databases = string.Join(',', client.ListDatabases().ToList());
                returnDict[port] = databases;
            }
            catch(Exception e) {
                returnDict[port] = null;
            }
        }

        return returnDict;
    }
}