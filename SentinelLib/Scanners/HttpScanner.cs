using SentinelLib.Models;
using SentinelLib.Models.ScannerParams;

namespace SentinelLib.Scanners;

public class HttpScanner<TEnum> : AbstractScanner<HttpScannerParams<TEnum>, TEnum> where TEnum : Enum {
    public HttpScanner(HttpScannerParams<TEnum> scannerParams) : base(scannerParams) {
        Ports = scannerParams.Ports;
    }

    protected override List<int> Ports { get; }

    public override async Task<Dictionary<int, Response>> Scan() {
        Dictionary<int, Response> returnDict = new();

        var openPorts = await ScanPorts();

        HttpClientHandler handler = new() {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5
        };
        HttpClient client = new(handler);
        foreach (var port in openPorts) {
            var url = $"https://{ScannerParams.Domain}:{port}";
            try {
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                var statusCode = (int)response.StatusCode;

                // Check status code according to settings
                switch (ScannerParams.StatusCodesGood) {
                    case HttpScannerParams<TEnum>.StatusCodes.Ok:
                        if (statusCode != 200)
                            goto Finished;
                        break;
                    case HttpScannerParams<TEnum>.StatusCodes.Under300:
                        if (statusCode >= 300)
                            goto Finished;
                        break;
                }

                HttpResponse httpResponse = new() {
                    StatusCode = statusCode,
                    TextResponse = response.Headers.ToString()
                };
                returnDict[port] = httpResponse;
            }
            catch (Exception e) {
                // ignored
            }

            // Ew goto
            Finished: ;
        }

        return returnDict;
    }
}