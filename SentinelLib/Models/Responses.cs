using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptOut)]
public class Response {
    [BsonIgnoreIfNull] public string? TextResponse { get; set; }

    [BsonIgnoreIfNull] public JToken? JsonResponse { get; set; }
}

[JsonObject(MemberSerialization.OptOut)]
public class HttpResponse : Response {
    public int StatusCode { get; set; }
}