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
    private readonly ResponseCallback _responseCallback;

    public delegate void ResponseCallback(ScannerOutput response);

    public Sentinel(ScannerProvider scannerProvider, int numWorkers, ResponseCallback responseCallback) {
        _scannerProvider = scannerProvider;
        _numWorkers = numWorkers;
        _responseCallback = responseCallback;
    }
    
    public void Start() {
        for (int i = 0; i < _numWorkers; i++) {
            var worker = new SentinelWorker(_work, _finished, _scannerProvider, _cancellationSource.Token, _responseCallback);
            var thread = new Thread(worker.Run);
            _workers.Add(thread);
            thread.Start();
        }
    }

    public void AddWork(ScannerParams scannerParams) {
        _work.Enqueue(scannerParams);
        lock (_work) {
            Monitor.Pulse(_work);
        }
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