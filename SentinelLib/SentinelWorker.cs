using System.Collections.Concurrent;

namespace SentinelLib; 

internal class SentinelWorker {
    private readonly ConcurrentQueue<ScannerParams> _workQueue;
    private readonly ConcurrentQueue<ScannerOutput> _finishedQueue;
    private readonly CancellationToken _cancellationToken;
    private readonly Sentinel.ResponseCallback _callback;
    private readonly ScannerProvider _scannerProvider;
    public SentinelWorker(ConcurrentQueue<ScannerParams> workQueue,
        ConcurrentQueue<ScannerOutput> finishedQueue,
        ScannerProvider scannerProvider,
        CancellationToken cancellationToken,
        Sentinel.ResponseCallback _callback) {
        _workQueue = workQueue;
        _cancellationToken = cancellationToken;
        this._callback = _callback;
        _scannerProvider = scannerProvider;
        _finishedQueue = finishedQueue;
    }
    
    public void Run() {
        while (true) {
            lock (_workQueue) {
                Monitor.Wait(_workQueue);
            }

            if (_cancellationToken.IsCancellationRequested) return;

            if (_workQueue.TryDequeue(out var work)) {
                var scanner = _scannerProvider.Instantiate(work);
                var output = new ScannerOutput {
                    Responses = scanner.Scan(),
                    InputParams = work
                };
                _finishedQueue.Enqueue(output);
                _callback(output);
            }
        }
    }
}