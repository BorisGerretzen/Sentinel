using Newtonsoft.Json;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptIn)]
public class HttpScannerParams : ScannerParams {
    /// <summary>
    ///     Ports to scan
    /// </summary>
    [JsonProperty] public List<int> Ports;

    public HttpScannerParams(
        string domain,
        ServiceType serviceType,
        List<int> ports) : base(domain, serviceType) {
        Ports = ports;
    }

    public HttpScannerParams(
        string domain,
        ServiceType serviceType) : this(domain, serviceType, new List<int> { 80, 8080, 8081 }) { }

    public HttpScannerParams(
        ScannerParams scannerParams,
        List<int> ports) : this(scannerParams.Domain, scannerParams.ServiceType, ports) { }
}