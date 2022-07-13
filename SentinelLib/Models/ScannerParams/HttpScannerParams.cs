using Newtonsoft.Json;
using SentinelLib.Scanners;

namespace SentinelLib.Models.ScannerParams;

[JsonObject(MemberSerialization.OptOut)]
public class HttpScannerParams<TEnum> : StandardScannerParams<TEnum> where TEnum : Enum {
    public enum StatusCodes {
        Ok,
        Under300,
        All
    }

    /// <summary>
    ///     Ports to scan
    /// </summary>
    public List<int> Ports;

    /// <summary>
    ///     Response codes that are seen as successful.
    /// </summary>
    public StatusCodes StatusCodesGood;

    /// <summary>
    ///     Parameters for an <see cref="HttpScanner{TEnum}" />.
    /// </summary>
    /// <param name="domain">The domain to scan.</param>
    /// <param name="serviceType">The service type of the domain.</param>
    /// <param name="ports">A list of ports to scan.</param>
    /// <param name="statusCodesGood">Which status codes will be regarded as a successful connection attempt.</param>
    public HttpScannerParams(string domain, TEnum serviceType, List<int> ports, StatusCodes statusCodesGood = StatusCodes.Ok) : base(domain, serviceType) {
        Ports = ports;
        StatusCodesGood = statusCodesGood;
    }

    public HttpScannerParams(string domain, TEnum serviceType) : this(domain, serviceType, new List<int> { 80, 8080, 8081 }) { }
    public HttpScannerParams(StandardScannerParams<TEnum> scannerParams, List<int> ports) : this(scannerParams.Domain, scannerParams.ServiceType, ports) { }
}