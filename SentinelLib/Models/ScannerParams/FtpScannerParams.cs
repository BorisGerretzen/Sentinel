namespace SentinelLib.Models.ScannerParams;

public class FtpScannerParams<TEnum> : StandardScannerParams<TEnum> where TEnum : Enum {
    /// <summary>
    ///     If empty response should be seen as successful.
    /// </summary>
    public readonly bool ShowEmpty;

    /// <summary>
    ///     Scanner parameters for the FTP scanner.
    ///     <remarks>An empty response is a response that only contains the directories '.' and '..'.</remarks>
    /// </summary>
    /// <param name="domain">The domain to scan.</param>
    /// <param name="serviceType">The service type of this scanner.</param>
    /// <param name="showEmpty">True if empty response should be seen as successful.</param>
    public FtpScannerParams(string domain, TEnum serviceType, bool showEmpty = false) : base(domain, serviceType) {
        ShowEmpty = showEmpty;
    }
}