using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SentinelLib.Models;

/// <summary>
///     Object that contains a response from a service.
///     <remarks>
///         Either <see cref="JsonResponse" /> or <see cref="TextResponse" /> is filled, not both. It depends on which
///         scanner you use.
///     </remarks>
/// </summary>
[JsonObject(MemberSerialization.OptOut)]
public class Response {
    /// <summary>
    ///     Text response from the connection attempt.
    /// </summary>
    [BsonIgnoreIfNull]
    public string? TextResponse { get; set; }

    /// <summary>
    ///     Error response from the connection attempt.
    /// </summary>
    [BsonIgnoreIfNull]
    public string? ErrorResponse { get; set; }

    /// <summary>
    ///     Json response from the connection attempt.
    /// </summary>
    [BsonIgnoreIfNull]
    public JToken? JsonResponse { get; set; }
}

[JsonObject(MemberSerialization.OptOut)]
public class HttpResponse : Response {
    /// <summary>
    ///     Status code from the connection attempt.
    /// </summary>
    public int StatusCode { get; set; }
}