using Newtonsoft.Json;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptOut)]
public class HttpScannerParams : ScannerParams {
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
    /// Response codes that are seen as successful.
    /// </summary>
    public StatusCodes StatusCodesGood;
    
    public HttpScannerParams(string domain, ServiceType serviceType, List<int> ports, StatusCodes statusCodesGood=StatusCodes.Ok) : base(domain, serviceType) {
        Ports = ports;
        StatusCodesGood = statusCodesGood;
    }
    public HttpScannerParams(
        string domain,
        ServiceType serviceType) : this(domain, serviceType, new List<int> { 80, 8080, 8081 }) { }

    public HttpScannerParams(
        ScannerParams scannerParams,
        List<int> ports) : this(scannerParams.Domain, scannerParams.ServiceType, ports) { }


}