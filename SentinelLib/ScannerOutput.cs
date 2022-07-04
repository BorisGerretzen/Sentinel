using Newtonsoft.Json;

namespace SentinelLib;

[JsonObject(MemberSerialization.OptIn)]
public class ScannerOutput {
    [JsonProperty]
    public ScannerParams InputParams { get; set; }
    [JsonProperty]
    public Dictionary<int, string?> Responses { get; set; }
}