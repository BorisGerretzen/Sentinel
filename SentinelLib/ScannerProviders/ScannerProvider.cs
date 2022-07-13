using SentinelLib.Models.ScannerParams;
using SentinelLib.Scanners;

namespace SentinelLib.ScannerProviders;

public class ScannerProvider<TScannerParams, TEnum> where TScannerParams : StandardScannerParams<TEnum> where TEnum : Enum {
    /// <summary>
    ///     Creator for an <see cref="IScanner" /> from generic <see cref="StandardScannerParams{TEnum}" />.
    /// </summary>
    public delegate IScanner Creator(TScannerParams para);


    private readonly Dictionary<TEnum, Creator> _registeredScanners = new();

    /// <summary>
    ///     Function that will return an <see cref="InvalidOperationException" /> to throw in case of invalid
    ///     <see cref="TScannerParams" />.
    /// </summary>
    protected readonly Func<InvalidOperationException> InvalidScannerParams = () => new InvalidOperationException("Invalid ScannerParams provided. Use the correct type required for the scanner.");

    /// <summary>
    ///     Instantiates a new scanner according to the name presented.
    ///     Name is case insensitive.
    /// </summary>
    /// <param name="scannerParams">Parameters for invoking the scanner.</param>
    /// <returns>Instantiated scanner</returns>
    public IScanner Instantiate(TScannerParams scannerParams) {
        if (!_registeredScanners.ContainsKey(scannerParams.ServiceType))
            throw new ArgumentException($"Cannot instantiate scanner of type '{scannerParams.ServiceType}' because it is not registered.");
        return _registeredScanners[scannerParams.ServiceType].Invoke(scannerParams);
    }

    /// <summary>
    ///     Register a new scanner in the provider.
    /// </summary>
    /// <param name="serviceType">The service of the scanner.</param>
    /// <param name="creator">Creator of the scanner.</param>
    public void Register(TEnum serviceType, Creator creator) {
        if (_registeredScanners.ContainsKey(serviceType))
            throw new ArgumentException($"ServiceType {serviceType} is already registered.");
        _registeredScanners[serviceType] = creator;
    }
}