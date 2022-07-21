namespace Sentinel;

public class AppSettings {
    public enum ExportTypes {
        Mongo,
        File,
        Both
    }

    /// <summary>
    ///     Connection string used to connect to the database where the scan results will be stored.
    /// </summary>
    public string? MongoConnectionString { get; set; } = "mongodb://127.0.0.1:27017/?directConnection=true";

    /// <summary>
    ///     Database where the scan results will be stored.
    /// </summary>
    public string? MongoDatabase { get; set; } = "Scans";

    /// <summary>
    ///     Collection where the scan results will be stored.
    /// </summary>
    public string? MongoCollection { get; set; } = "ScannerOutput";

    /// <summary>
    ///     How to export the scan results, choose from <see cref="ExportTypes" />.
    ///     <remarks>
    ///         Choose from the following: <br />
    ///         - Mongo <br />
    ///         - File <br />
    ///         - Both
    ///     </remarks>
    /// </summary>
    public ExportTypes ExportType { get; set; } = ExportTypes.Mongo;
}