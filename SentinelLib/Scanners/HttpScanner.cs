using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class HttpScanner : Scanner {
    public HttpScanner(HttpScannerParams scannerParams) : base(scannerParams) {
        Ports = scannerParams.Ports;
    }

    protected override List<int> Ports { get; }

    public override async Task<Dictionary<int, Response>> Scan() {
        var scannerParams = (HttpScannerParams)ScannerParams;
        var returnDict = new Dictionary<int, Response>();

        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        
        var handler = new HttpClientHandler {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5
        };
        var client = new HttpClient(handler);
        foreach (var port in openPorts) {
            var url = $"https://{scannerParams.Domain}:{port}";
            try {
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                int statusCode = (int)response.StatusCode;
                
                // Check status code according to settings
                switch (scannerParams.StatusCodesGood) {
                    case HttpScannerParams.StatusCodes.Ok:
                        if(statusCode != 200)
                            goto Finished;
                        break;
                    case HttpScannerParams.StatusCodes.Under300:
                        if(statusCode >= 300)
                            goto Finished;
                        break;
                }

                var httpResponse = new HttpResponse {
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