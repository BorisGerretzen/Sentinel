using System.Runtime.Caching;
using SentinelLib.Models;
using SentinelLib.Scanners;

namespace SentinelLib;

public class Sentinel {
    public delegate Task ResponseCallback(ScannerOutput response);

    /// <summary>
    ///     True if caching should be enabled.
    /// </summary>
    private readonly bool _cache;

    /// <summary>
    ///     Function that should be called when an accessible domain has been identified.
    /// </summary>
    private readonly ResponseCallback _responseCallback;

    /// <summary>
    ///     Provider for the various scanner types.
    /// </summary>
    private readonly ScannerProvider _scannerProvider;

    /// <summary>
    ///     Semaphore for limiting the maximum number of concurrent connections.
    /// </summary>
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    ///     The cache that holds the recently scanned domains.
    /// </summary>
    private readonly MemoryCache _workCache = new("work");

    /// <summary>
    ///     Creates a new sentinel object.
    ///     <remarks>
    ///         Because domains can appear in multiple certificate transparency chains, domain caching can be enabled to
    ///         ignore duplicates.
    ///     </remarks>
    /// </summary>
    /// <param name="scannerProvider">The <see cref="ScannerProvider" /> to use.</param>
    /// <param name="responseCallback">
    ///     Method with signature of <see cref="ResponseCallback" /> that gets called when a domain
    ///     has been successfully scanned to be accessible.
    /// </param>
    /// <param name="cache">True if domain caching should be used.</param>
    public Sentinel(ScannerProvider scannerProvider, ResponseCallback responseCallback, bool cache = true) {
        _scannerProvider = scannerProvider;
        _responseCallback = responseCallback;
        _semaphore = new SemaphoreSlim(200, 200);
        _cache = cache;
    }

    /// <summary>
    ///     Adds work to the sentinel.
    /// </summary>
    /// <param name="scannerParams">The work to be done.</param>
    public void AddWork(ScannerParams scannerParams) {
        Task.Run(async () => await Run(scannerParams));
    }

    /// <summary>
    ///     Safely prints content to the output.
    /// </summary>
    /// <param name="content">The content to print.</param>
    private void PrintSafe(string content) {
        lock (_scannerProvider) {
            Console.WriteLine(content);
        }
    }

    /// <summary>
    ///     Handles caching. Checks if the supplied domain is in the cache, if it is return false.
    ///     Otherwise, it is added to the cache and true is returned.
    ///     <remarks>If the cache is disabled, this method always returns true.</remarks>
    /// </summary>
    /// <param name="domain">The domain.</param>
    private bool DoCache(string domain) {
        if (!_cache) return true;
        if (_workCache.Contains(domain)) {
            PrintSafe($"[{domain}] {"Domain was found in cache, skipping",-30}");
            return false;
        }

        _workCache.Add(domain, "aa", DateTimeOffset.Now.AddMinutes(2));
        return true;
    }

    /// <summary>
    ///     Instantiates a scanner according to the <see cref="ServiceType" /> and runs the scan.
    /// </summary>
    /// <param name="work">The work to be done.</param>
    private async Task Run(ScannerParams work) {
        if (!DoCache(work.Domain)) return;

        await _semaphore.WaitAsync();
        Scanner scanner = _scannerProvider.Instantiate(work);
        PrintSafe($"[{work.Domain}] {$"Starting scan for type '{work.ServiceType}'",-20}");
        var responses = await scanner.Scan();
        _semaphore.Release();

        PrintSafe($"[{work.Domain}] {"Scan completed",-30}");
        if (responses.Count == 0) return;
        PrintSafe($"[{work.Domain}] {$"Accessible on {responses.Count} port{(responses.Count > 1 ? "s" : string.Empty)}",-30}");
        ScannerOutput output = new() {
            Responses = responses,
            InputParams = work
        };
        await _responseCallback(output);
    }
}