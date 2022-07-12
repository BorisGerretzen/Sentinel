using System.Net;
using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class FtpScanner : Scanner {
    public FtpScanner(ScannerParams scannerParams) : base(scannerParams) { }
    protected override List<int> Ports => new() { 21 };

    public override async Task<Dictionary<int, Response>> Scan() {
        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        Dictionary<int, Response> returnDict = new();
        var useSsl = true;

        Retry: // Ugly goto but seems like a good solution here
        foreach (var port in openPorts)
            try {
                // FTP request
#pragma warning disable SYSLIB0014
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ScannerParams.Domain}");
#pragma warning restore SYSLIB0014
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential("anonymous", $"anonymous@{ScannerParams.Domain}");
                request.EnableSsl = useSsl;
                request.UsePassive = true;

                // Get response async and read
                FtpWebResponse ftpResponse = await Task.Factory.FromAsync(request.BeginGetResponse, result => (FtpWebResponse)request.EndGetResponse(result), null);
                Stream stream = ftpResponse.GetResponseStream();
                StreamReader reader = new(stream);
                var content = await reader.ReadToEndAsync();

                // Return
                returnDict[port] = new Response {
                    TextResponse = content
                };
            }
            catch (Exception e) {
                // Retry with SSL disabled
                if (e.Message.Contains("RemoteCertificateNameMismatch") && useSsl) {
                    useSsl = false;
                    goto Retry;
                }

                if (e.Message.Contains("RemoteCertificateNameMismatch")) break; // prevent infinite loop

                // Only log special errors
                if (e.Message.Contains("Not logged in")) break;
                returnDict[port] = new Response {
                    ErrorResponse = e.Message
                };
            }

        return returnDict;
    }
}