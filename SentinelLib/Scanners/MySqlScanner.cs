﻿using System.Data.Common;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class MySqlScanner : Scanner {
    public MySqlScanner(ScannerParams scannerParams) : base(scannerParams) { }

    protected override List<int> Ports => new() { 3306 };

    public override async Task<Dictionary<int, Response>> Scan() {
        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key);
        Dictionary<int, Response> returnDict = new();
        JObject jobject = new();
        var enumerable = openPorts.ToList();
        foreach (var username in new List<string> { "test", "root" })
        foreach (var port in enumerable)
            try {
                // Connect to server
                var cs = $@"server={ScannerParams.Domain}:{port};userid={username};";
                using MySqlConnection conn = new(cs);
                await conn.OpenAsync();
                jobject.Add("version", conn.ServerVersion);
                returnDict[port] = new Response {
                    JsonResponse = jobject
                };

                // Get databases
                MySqlCommand cmd = new("SHOW DATABASES;", conn);
                DbDataReader reader = await cmd.ExecuteReaderAsync();
                List<string> databases = new();
                while (await reader.ReadAsync()) databases.Add(reader.GetString(0));

                jobject = new JObject {
                    { "version", conn.ServerVersion },
                    { "databases", JArray.FromObject(databases) }
                };
                returnDict[port] = new Response {
                    JsonResponse = jobject
                };
            }
            catch (Exception e) {
                // do nothing
            }

        return returnDict;
    }
}