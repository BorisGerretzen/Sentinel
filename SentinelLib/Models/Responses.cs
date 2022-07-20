using MongoDB.Bson;
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
    [BsonIgnore]
    public JObject? JsonResponse { get; set; }

    /// <summary>
    /// Bson version of <see cref="JsonResponse"/>, can be stored in MongoDB.
    /// </summary>
    [BsonIgnoreIfNull]
    [JsonIgnore]
    public object? BsonResponse {
        get => JsonResponse == null ? null : BsonTypeMapper.MapToBsonValue(ToObject(JsonResponse));
        set => JsonResponse = value == null ? null : JObject.FromObject(value);
    }

    /// <summary>
    ///     Used to convert a JToken to a .NET object.
    ///     This can then be used to convert to a Bson value to use with MongoDB.
    /// </summary>
    /// <param name="token">JToken to convert.</param>
    /// <returns>Native .net object.</returns>
    protected static object? ToObject(JToken token) {
        switch (token.Type) {
            case JTokenType.Object:
                return token.Children<JProperty>()
                    .ToDictionary(prop => prop.Name,
                        prop => ToObject(prop.Value));

            case JTokenType.Array:
                return token.Select(ToObject).ToList();

            default:
                return ((JValue)token).Value;
        }
    }
}

[JsonObject(MemberSerialization.OptOut)]
public class HttpResponse : Response {
    /// <summary>
    ///     Status code from the connection attempt.
    /// </summary>
    public int StatusCode { get; set; }
}