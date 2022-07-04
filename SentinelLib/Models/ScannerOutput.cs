using Newtonsoft.Json;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptIn)]
public class ScannerOutput {
    [JsonProperty] public ScannerParams? InputParams { get; set; }
    [JsonProperty] public Dictionary<int, Response>? Responses { get; set; }
}