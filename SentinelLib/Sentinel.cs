using SentinelLib.Models;

namespace SentinelLib;

public class Sentinel {
    public delegate void ResponseCallback(ScannerOutput response);

    private readonly ResponseCallback _responseCallback;
    private readonly ScannerProvider _scannerProvider;
    private readonly SemaphoreSlim _semaphore;
    public Sentinel(ScannerProvider scannerProvider, ResponseCallback responseCallback) {
        _scannerProvider = scannerProvider;
        _responseCallback = responseCallback;
        _semaphore = new SemaphoreSlim(200, 200);
    }

    public void AddWork(ScannerParams scannerParams) {
        //ThreadPool.QueueUserWorkItem(Run, scannerParams);
        Task.Run(() => Run(scannerParams));
        //Task.Run(() => Run(scannerParams));
    }

    private async void Run(object? state) {
        await _semaphore.WaitAsync();
        var work = state as ScannerParams;
        var scanner = _scannerProvider.Instantiate(work);
        lock (_scannerProvider) {
            Console.WriteLine(work.Domain);
        }
        var responses = await scanner.Scan();
        _semaphore.Release();
        if (responses.Count == 0) return;
        var output = new ScannerOutput {
            Responses = responses,
            InputParams = work
        };
        _responseCallback(output);
    }
}