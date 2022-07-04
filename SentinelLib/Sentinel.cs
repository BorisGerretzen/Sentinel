using SentinelLib.Models;

namespace SentinelLib;

public class Sentinel {
    public delegate void ResponseCallback(ScannerOutput response);

    private readonly ResponseCallback _responseCallback;
    private readonly ScannerProvider _scannerProvider;

    public Sentinel(ScannerProvider scannerProvider, ResponseCallback responseCallback) {
        _scannerProvider = scannerProvider;
        _responseCallback = responseCallback;
    }

    public void AddWork(ScannerParams scannerParams) {
        Task.Run(() => Run(scannerParams));
    }

    private async Task Run(ScannerParams work) {
        var scanner = _scannerProvider.Instantiate(work);
        var responses = await scanner.Scan();
        if (responses.Count == 0) return;
        var output = new ScannerOutput {
            Responses = responses,
            InputParams = work
        };
        _responseCallback(output);
    }
}