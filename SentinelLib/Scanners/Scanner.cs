using System.Net.Sockets;
using SentinelLib.Models;

namespace SentinelLib.Scanners;

public abstract class Scanner {
    protected readonly ScannerParams ScannerParams;

    /// <summary>
    ///     Creates a new scanner object.
    /// </summary>
    /// <param name="scannerParams">Parameters for the host to scan.</param>
    protected Scanner(ScannerParams scannerParams) {
        ScannerParams = scannerParams;
    }

    /// <summary>
    ///     The ports to scan for a host of this scanner's type.
    /// </summary>
    protected abstract List<int> Ports { get; }

    /// <summary>
    ///     Scans a list of ports for the domain in this object's <see cref="Models.ScannerParams" />.
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

    /// <summary>
    ///     Scans the host specified in this object's <see cref="Models.ScannerParams" />
    /// </summary>
    /// <returns>Dictionary containing the port and the status of the service.</returns>
    public abstract Task<Dictionary<int, Response>> Scan();
}