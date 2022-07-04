using Newtonsoft.Json;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptOut)]
public class Response {
    public string? TextResponse { get; set; }
}

[JsonObject(MemberSerialization.OptOut)]
public class HttpResponse : Response {
    public int StatusCode { get; set; }
}