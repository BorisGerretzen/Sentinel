﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SentinelLib.Models.ScannerParams;

/// <summary>
///     Standard parameters that are passed to a scanner.
/// </summary>
/// <typeparam name="TEnum">Enum to indicate service type.</typeparam>
[JsonObject(MemberSerialization.OptOut)]
public class StandardScannerParams<TEnum> where TEnum : Enum {
    /// <summary>
    ///     This function should be called when open ports are found.
    /// </summary>
    [JsonIgnore] [BsonIgnore] internal Sentinel<TEnum>.OpenPortCallback? OpenPortCallback;

    /// <summary>
    ///     Creates a new <see cref="StandardScannerParams{TEnum}" /> used to put a scanner to work.
    /// </summary>
    /// <param name="domain">The domain to scan.</param>
    /// <param name="serviceType">The service that will be scanned.</param>
    public StandardScannerParams(string domain, TEnum serviceType) {
        Domain = domain;
        ServiceType = serviceType;
    }

    /// <summary>
    ///     The domain to scan.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    ///     The service with which the connection attempt will be made.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [JsonConverter(typeof(StringEnumConverter))]
    public TEnum ServiceType { get; set; }
}