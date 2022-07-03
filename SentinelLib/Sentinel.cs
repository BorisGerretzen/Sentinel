using System.Collections.Concurrent;
using SentinelLib.Scanners;

namespace SentinelLib; 

public class Sentinel :IDisposable {
    private readonly ConcurrentQueue<ScannerParams> _work = new();
    private readonly ConcurrentQueue<ScannerOutput> _finished = new();
    private readonly List<Thread> _workers = new();
    private readonly ScannerProvider _scannerProvider;
    private readonly CancellationTokenSource _cancellationSource = new();
    private readonly int _numWorkers;
    
    public Sentinel(ScannerProvider scannerProvider, int numWorkers) {
        _scannerProvider = scannerProvider;
        _numWorkers = numWorkers;
    }
    
    public void Start() {
        for (int i = 0; i < _numWorkers; i++) {
            var worker = new SentinelWorker(_work, _finished, _scannerProvider, _cancellationSource.Token);
            _workers.Add(new Thread(worker.Run));
        }
    }
    
    public void AddWork(ScannerParams scannerParams) {
        _work.Enqueue(scannerParams);
    }
    
    #region Disposing
    public void Stop() {
        Dispose();
    }

    private void ReleaseUnmanagedResources() {
        foreach (var worker in _workers) {
            worker.Join();
        }
    }

    private void Dispose(bool disposing) {
        Stop();
        ReleaseUnmanagedResources();
        if (disposing) {
            _cancellationSource.Dispose();
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}