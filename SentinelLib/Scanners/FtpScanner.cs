using System.Net;
using SentinelLib.Models;
using SentinelLib.Models.ScannerParams;

namespace SentinelLib.Scanners;

public class FtpScanner<TEnum> : AbstractScanner<FtpScannerParams<TEnum>, TEnum> where TEnum : Enum {
    public FtpScanner(FtpScannerParams<TEnum> scannerParams) : base(scannerParams) { }

    protected override List<int> Ports => new() { 21, 22 };

    /// <summary>
    ///     Scans an FTP server.
    /// </summary>
    /// <param name="ssl">True if SSL should be enabled.</param>
    private async Task<Response> ScanFtp(bool ssl) {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ScannerParams.Domain}");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.Credentials = new NetworkCredential("anonymous", $"anonymous@{ScannerParams.Domain}");
        request.UsePassive = true;
        request.KeepAlive = true;
        if (ssl) {
            request.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = (_, _, _, _) => true; // Accept all certificates
        }

        // Get response async and read
        FtpWebResponse ftpResponse = await Task.Factory.FromAsync(request.BeginGetResponse, result => (FtpWebResponse)request.EndGetResponse(result), null);
        Stream stream = ftpResponse.GetResponseStream();
        StreamReader reader = new(stream);
        var content = await reader.ReadToEndAsync();

        if (content == ".\r\n..\r\n") { }

        // Return
        return new Response {
            TextResponse = content
        };
    }

    public override async Task<Dictionary<int, Response>> Scan() {
        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        Dictionary<int, Response> returnDict = new();

        foreach (var port in openPorts)
            try {
                Response? result = null;
                if (port == 22) result = await ScanFtp(true);
                if (port == 21) result = await ScanFtp(false);

                returnDict[port] = result ?? throw new InvalidOperationException($"Unknown port {port} for this service");
            }
            catch (Exception e) {
                // do nothing
            }

        return returnDict;
    }
}