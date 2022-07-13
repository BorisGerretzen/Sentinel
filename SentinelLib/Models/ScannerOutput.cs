using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;

namespace SentinelLib.Models;

/// <summary>
///     Output object from a scanner.
/// </summary>
/// <typeparam name="TEnum">Enum to indicate service type.</typeparam>
[JsonObject(MemberSerialization.OptOut)]
public class ScannerOutput<TEnum> where TEnum : Enum {
    /// <summary>
    ///     The input parameters that resulted in the scan result.
    /// </summary>
    public StandardScannerParams<TEnum>? InputParams { get; set; }

    /// <summary>
    ///     A map of ports and their respective responses.
    /// </summary>
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    [BsonIgnoreIfNull]
    public Dictionary<int, Response>? Responses { get; set; }
}