using MongoDB.Driver;
using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class MongoScanner : Scanner {
    public MongoScanner(ScannerParams scannerParams) : base(scannerParams) { }
    protected override List<int> Ports => new() { 27017 };

    public override async Task<Dictionary<int, Response>> Scan() {
        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key);
        Dictionary<int, Response> returnDict = new();
        foreach (var port in openPorts)
            try {
                var client = new MongoClient($"mongodb://{_scannerParams.Domain}:{port}");
                var databases = (await client.ListDatabasesAsync()).ToList();
                returnDict[port] = new Response {
                    TextResponse = string.Join(',', databases)
                };
            }
            catch {
                // ignored
            }

        return returnDict;
    }
}