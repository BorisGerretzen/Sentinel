using Certstream;
using MongoDB.Driver;
using Newtonsoft.Json;
using SentinelLib.Models;
using SentinelLib.Models.ScannerParams;
using SentinelLib.ScannerProviders;

namespace Sentinel;

public class Main {
    private readonly AppSettings _appSettings;
    private readonly Sentinel<ServiceType> _sentinel;
    private readonly IMongoCollection<ScannerOutput<ServiceType>>? _storageCollection;

    public Main(AppSettings appSettings) {
        _appSettings = appSettings;

        // Init MongoDB connection
        if (_appSettings.ExportType is AppSettings.ExportTypes.Both or AppSettings.ExportTypes.Mongo) {
            MongoClient storageClient = new(_appSettings.MongoConnectionString);
            IMongoDatabase storageDb = storageClient.GetDatabase(_appSettings.MongoDatabase);
            _storageCollection = storageDb.GetCollection<ScannerOutput<ServiceType>>(_appSettings.MongoCollection);
        }

        _sentinel = new Sentinel<ServiceType>(DefaultScannerProvider.Provider, ResponseCallback);
    }

    public void Start() {
        CertstreamClient client = new(-1);
        client.CertificateIssued += NewCert;

        Console.WriteLine("Sentinel has been initialized");
        client.Start();
        Console.ReadLine();
        client.Stop();
    }

    /// <summary>
    ///     Called when a service is successfully scanned.
    /// </summary>
    /// <param name="param">Output from the scan.</param>
    private async Task ResponseCallback(ScannerOutput<ServiceType> param) {
        // File export
        if (_appSettings.ExportType is AppSettings.ExportTypes.Both or AppSettings.ExportTypes.File) {
            var json = JsonConvert.SerializeObject(
                param,
                Formatting.Indented,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
            await File.WriteAllTextAsync($"{param.InputParams?.Domain}.json", json);
        }

        // Mongo export
        if (_appSettings.ExportType is AppSettings.ExportTypes.Both or AppSettings.ExportTypes.Mongo)
            try {
                if (_storageCollection is null) throw new NullReferenceException("Storage collection not initialized.");
                await _storageCollection.InsertOneAsync(param);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
    }

    /// <summary>
    ///     Event handler for when a new certificate is received from Certstream.
    /// </summary>
    /// <param name="sender">Certstream object.</param>
    /// <param name="cert">Certificate received.</param>
    private void NewCert(object? sender, LeafCert cert) {
        foreach (var domain in cert.AllDomains) {
            if (string.IsNullOrEmpty(domain)) continue;

            // get label
            var split = domain.Split('.');
            if (split.Length <= 2) continue;
            var label = split[0];

            // Get ServiceType
            ServiceType serviceType = Helpers.StringToServiceType(label);
            if (serviceType == ServiceType.None) continue;
            switch (serviceType) {
                case ServiceType.ElasticSearch:
                    _sentinel.AddWork(new HttpScannerParams<ServiceType>(domain, serviceType, new List<int> { 9200 }));
                    break;
                case ServiceType.MongoExpress:
                    _sentinel.AddWork(new HttpScannerParams<ServiceType>(domain, serviceType));
                    break;
                case ServiceType.Ftp:
                    _sentinel.AddWork(new FtpScannerParams<ServiceType>(domain, serviceType));
                    break;
                default:
                    _sentinel.AddWork(new StandardScannerParams<ServiceType>(domain, serviceType));
                    break;
            }
        }
    }
}