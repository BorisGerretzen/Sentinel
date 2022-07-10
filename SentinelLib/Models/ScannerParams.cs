using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptOut)]
public class ScannerParams {
    public string Domain;

    [JsonConverter(typeof(StringEnumConverter))]
    public ServiceType ServiceType;

    public ScannerParams(string domain, ServiceType serviceType) {
        Domain = domain;
        ServiceType = serviceType;
    }
}