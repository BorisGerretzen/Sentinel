using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptOut)]
public class Response {
    public string? TextResponse { get; set; }
    public JToken? JsonResponse { get; set; }
}

[JsonObject(MemberSerialization.OptOut)]
public class HttpResponse : Response {
    public int StatusCode { get; set; }
}