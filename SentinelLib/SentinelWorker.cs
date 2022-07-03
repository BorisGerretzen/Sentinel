using System.Collections.Concurrent;

namespace SentinelLib; 

internal class SentinelWorker {
    private readonly ConcurrentQueue<ScannerParams> _workQueue;
    private readonly ConcurrentQueue<ScannerOutput> _finishedQueue;
    private readonly CancellationToken _cancellationToken;
    private readonly ScannerProvider _scannerProvider;
    public SentinelWorker(ConcurrentQueue<ScannerParams> workQueue,
        ConcurrentQueue<ScannerOutput> finishedQueue,
        ScannerProvider scannerProvider,
        CancellationToken cancellationToken) {
        _workQueue = workQueue;
        _cancellationToken = cancellationToken;
        _scannerProvider = scannerProvider;
        _finishedQueue = finishedQueue;
    }
    
    public void Run() {
        while (true) {
            if (_cancellationToken.IsCancellationRequested) return;

            if (_workQueue.TryDequeue(out var work)) {
                var scanner = _scannerProvider.Instantiate(work);
                var output = new ScannerOutput {
                    Responses = scanner.Scan()
                };
                _finishedQueue.Enqueue(output);
            }
        }
    }
}