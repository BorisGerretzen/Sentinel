using SentinelLib.Models.ScannerParams;
using SentinelLib.Scanners;

namespace SentinelLib.ScannerProviders;

/// <summary>
///     Default scanner provider, contains all scanners in <see cref="SentinelLib" />
/// </summary>
public class DefaultScannerProvider : ScannerProvider<StandardScannerParams<ServiceType>, ServiceType> {
    /// <summary>
    ///     The Default scanner provider, contains all scanners in <see cref="SentinelLib" />
    /// </summary>
    public static readonly DefaultScannerProvider Provider = new();

    private DefaultScannerProvider() {
        Register(ServiceType.Mongo, scannerParams => new MongoScanner<ServiceType>(scannerParams));
        Register(ServiceType.MySql, scannerParams => new MySqlScanner<ServiceType>(scannerParams));
        Register(ServiceType.MongoExpress, scannerParams => new HttpScanner<ServiceType>(scannerParams as HttpScannerParams<ServiceType> ?? throw InvalidScannerParams()));
        Register(ServiceType.ElasticSearch, scannerParams => new HttpScanner<ServiceType>(scannerParams as HttpScannerParams<ServiceType> ?? throw InvalidScannerParams()));
        Register(ServiceType.Ftp, scannerParams => new FtpScanner<ServiceType>(scannerParams as FtpScannerParams<ServiceType> ?? throw InvalidScannerParams()));
    }
}