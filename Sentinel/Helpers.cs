using SentinelLib;

namespace Sentinel; 

public static class Helpers {
    public static ServiceType StringToServiceType(string label) {
        label = label.ToLower();
        switch (label) {
            case "radarr":
                return ServiceType.Sonarr;
            case "sonarr":
                return ServiceType.Sonarr;
            case "mongo":
            case "mongodb":
                return ServiceType.Mongo;
            case "mongoexpress":
            case "mongo-express":
                return ServiceType.MongoExpress;
            case "elastic":
            case "elasticsearch":
                return ServiceType.ElasticSearch;
            default:
                return ServiceType.None;
        }
    }
}