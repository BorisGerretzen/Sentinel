using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptIn)]
public class ScannerParams {
    [JsonProperty] public string Domain;

    [JsonProperty] [JsonConverter(typeof(StringEnumConverter))]
    public ServiceType ServiceType;

    public ScannerParams(string domain, ServiceType serviceType) {
        Domain = domain;
        ServiceType = serviceType;
    }
}