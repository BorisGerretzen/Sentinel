using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class HttpScanner : Scanner {
    public HttpScanner(HttpScannerParams scannerParams) : base(scannerParams) {
        Ports = scannerParams.Ports;
    }

    protected override List<int> Ports { get; }

    public override async Task<Dictionary<int, Response>> Scan() {
        HttpScannerParams scannerParams = (HttpScannerParams)ScannerParams;
        Dictionary<int, Response> returnDict = new();

        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        HttpClientHandler handler = new() {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5
        };
        HttpClient client = new(handler);
        foreach (var port in openPorts) {
            var url = $"https://{scannerParams.Domain}:{port}";
            try {
                HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                var statusCode = (int)response.StatusCode;

                // Check status code according to settings
                switch (scannerParams.StatusCodesGood) {
                    case HttpScannerParams.StatusCodes.Ok:
                        if (statusCode != 200)
                            goto Finished;
                        break;
                    case HttpScannerParams.StatusCodes.Under300:
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