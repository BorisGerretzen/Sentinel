using SentinelLib.Models;
using SentinelLib.Scanners;

namespace SentinelLib;

public sealed class ScannerProvider {
    /// <summary>
    ///     Default ScannerProvider
    /// </summary>
    public static ScannerProvider DefaultProvider = new();

    private readonly Dictionary<ServiceType, Func<ScannerParams, Scanner>> _registeredScanners = new();

    /// <summary>
    ///     Creates a new ScannerProvider that handles the instantiation of different scanners.
    /// </summary>
    private ScannerProvider() {
        Register(ServiceType.Mongo, scannerParams => new MongoScanner(scannerParams));
        Register(ServiceType.MongoExpress, scannerParams => new HttpScanner((HttpScannerParams)scannerParams));
        Register(ServiceType.ElasticSearch, scannerParams => new HttpScanner((HttpScannerParams)scannerParams));
        Register(ServiceType.MySql, scannerParams => new MySqlScanner(scannerParams));
    }

    /// <summary>
    ///     Instantiates a new scanner according to the name presented.
    ///     Name is case insensitive.
    /// </summary>
    /// <param name="scannerParams">Parameters for invoking the scanner.</param>
    /// <returns>Instantiated scanner</returns>
    public Scanner Instantiate(ScannerParams scannerParams) {
        if (!_registeredScanners.ContainsKey(scannerParams.ServiceType)) throw new ArgumentException($"Cannot instantiate scanner of type '{scannerParams.ServiceType}' because it is not registered.");
        return _registeredScanners[scannerParams.ServiceType].Invoke(scannerParams);
    }

    /// <summary>
    ///     Register a new scanner in the provider.
    /// </summary>
    /// <param name="serviceType">The service of the scanner.</param>
    /// <param name="creator">Creator of the scanner.</param>
    public void Register(ServiceType serviceType, Func<ScannerParams, Scanner> creator) {
        if (_registeredScanners.ContainsKey(serviceType))
            throw new ArgumentException($"ServiceType {serviceType} is already registered.");
        _registeredScanners[serviceType] = creator;
    }
}