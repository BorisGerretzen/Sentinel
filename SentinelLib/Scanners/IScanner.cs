using SentinelLib.Models;

namespace SentinelLib.Scanners;

public interface IScanner {
    public Task<Dictionary<int, Response>> Scan();
}