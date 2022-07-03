namespace SentinelLib; 

public class ScannerParams {
    public string Domain;
    public ServiceType ServiceType;
    public ScannerParams(string domain, ServiceType serviceType) {
        Domain = domain;
        ServiceType = serviceType;
    }
}