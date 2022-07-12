using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;

namespace SentinelLib.Models;

[JsonObject(MemberSerialization.OptOut)]
public class ScannerOutput {
    public ScannerParams? InputParams { get; set; }

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    [BsonIgnoreIfNull]
    public Dictionary<int, Response>? Responses { get; set; }
}