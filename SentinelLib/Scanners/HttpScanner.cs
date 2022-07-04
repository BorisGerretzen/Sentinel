using SentinelLib.Models;

namespace SentinelLib.Scanners;

public class HttpScanner : Scanner {
    public HttpScanner(HttpScannerParams scannerParams) : base(scannerParams) {
        Ports = scannerParams.Ports;
    }

    protected override List<int> Ports { get; }

    public override async Task<Dictionary<int, Response>> Scan() {
        var scannerParams = (HttpScannerParams)_scannerParams;
        var openPorts = (await ScanPorts()).Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        var client = new HttpClient();
        var returnDict = new Dictionary<int, Response>();
        foreach (var port in openPorts) {
            var url = $"http://{scannerParams.Domain}:{port}";
            try {
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                var httpResponse = new HttpResponse {
                    StatusCode = (int)response.StatusCode,
                    TextResponse = response.Headers.ToString()
                };
                returnDict[port] = httpResponse;
            }
            catch (Exception e) {
                // ignored
            }
        }

        return returnDict;
    }
}