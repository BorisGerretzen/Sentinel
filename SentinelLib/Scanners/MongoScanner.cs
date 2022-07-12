﻿using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class MongoScanner : Scanner {
    public MongoScanner(ScannerParams scannerParams) : base(scannerParams) { }
    protected override List<int> Ports => new() { 27017 };

    private JArray TransformData(List<BsonDocument> databases) {
        JArray jArray = new();
        foreach (BsonDocument database in databases) {
            var nativeObject = BsonTypeMapper.MapToDotNetValue(database);
            jArray.Add(JObject.FromObject(nativeObject));
        }

        return jArray;
    }

    public override async Task<Dictionary<int, Response>> Scan() {
        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key);
        Dictionary<int, Response> returnDict = new();
        foreach (var port in openPorts)
            try {
                MongoClient client = new($"mongodb://{ScannerParams.Domain}:{port}/db?directConnection=true");
                var databases = (await client.ListDatabasesAsync()).ToList();
                if (databases is not null)
                    returnDict[port] = new Response {
                        JsonResponse = TransformData(databases)
                    };
            }
            catch (Exception e) {
                // nothing
            }

        return returnDict;
    }
}