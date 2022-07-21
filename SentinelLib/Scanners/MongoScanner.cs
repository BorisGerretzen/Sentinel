using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SentinelLib.Models;
using SentinelLib.Models.ScannerParams;

namespace SentinelLib.Scanners;

public class MongoScanner<TEnum> : AbstractScanner<StandardScannerParams<TEnum>, TEnum> where TEnum : Enum {
    public MongoScanner(StandardScannerParams<TEnum> scannerParams) : base(scannerParams) { }
    protected override List<int> Ports => new() { 27017 };

    /// <summary>
    ///     Transforms a list of <see cref="BsonDocument" /> to a <see cref="JArray" /> that can be returned in a
    ///     <see cref="Response" />.
    /// </summary>
    /// <param name="databases">List of databases.</param>
    private JArray TransformData(List<BsonDocument> databases) {
        JArray jArray = new();
        foreach (BsonDocument database in databases) {
            var nativeObject = BsonTypeMapper.MapToDotNetValue(database);
            jArray.Add(JObject.FromObject(nativeObject));
        }

        return jArray;
    }

    public override async Task<Dictionary<int, Response>> Scan() {
        var openPorts = await ScanPorts();
        Dictionary<int, Response> returnDict = new();
        foreach (var port in openPorts)
            try {
                MongoClient client = new($"mongodb://{ScannerParams.Domain}:{port}/db?directConnection=true");
                var databases = (await client.ListDatabasesAsync()).ToList();
                if (databases == null) continue;

                if (databases is not null)
                    returnDict[port] = new Response {
                        JsonResponse = new JObject { { "databases", TransformData(databases) } }
                    };
            }
            catch (Exception e) {
                // nothing
            }

        return returnDict;
    }
}