using System.Net.Sockets;
using SentinelLib.Models;
using SentinelLib.Models.ScannerParams;

namespace SentinelLib.Scanners;

public abstract class AbstractScanner<TScannerParams, TEnum> : IScanner where TScannerParams : StandardScannerParams<TEnum> where TEnum : Enum {
    protected readonly TScannerParams ScannerParams;

    /// <summary>
    ///     Creates a new scanner object.
    /// </summary>
    /// <param name="scannerParams">Parameters for the host to scan.</param>
    protected AbstractScanner(TScannerParams scannerParams) {
        ScannerParams = scannerParams;
    }

    /// <summary>
    ///     The ports to scan for a host of this scanner's type.
    /// </summary>
    protected abstract List<int> Ports { get; }

    /// <summary>
    ///     Scans the host specified in this object's <see cref="StandardScannerParams{TEnum}" />
    /// </summary>
    /// <returns>Dictionary containing the port and the status of the service.</returns>
    public abstract Task<Dictionary<int, Response>> Scan();

    /// <summary>
    ///     Scans a list of ports for the domain in this object's <see cref="StandardScannerParams{TEnum}" />.
    /// </summary>
    /// <returns>Dictionary containing the port and the status (open/closed).</returns>
    protected async Task<Dictionary<int, bool>> ScanPorts() {
        Dictionary<int, bool> returnDict = new();

        foreach (var port in Ports) {
            using TcpClient client = new();
            try {
                await client.ConnectAsync(ScannerParams.Domain, port);
                returnDict[port] = true;
            }
            catch {
                returnDict[port] = false;
            }
        }

        return returnDict;
    }
}