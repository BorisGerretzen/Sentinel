# Sentinel
Sentinel monitors certificate transparency logs and looks for services that have not configured authentication correctly.

I recommend using a vpn while running this service.

Currently, the following services are supported:
- MongoDB
- Mongo-Express
- Elasticsearch
- Mysql
- It is possible to add more services/labels by registering them in SentinelLib.

## How it works
Sentinel uses [certstream](https://certstream.calidog.io) to get a live feed of certificates added to CT logs. It extracts the domain names from these certificates and checks the first label of these domain names.
If the label is one of the recognized labels, a connection is attempted with a client of the corresponding service.
For example, `mongo.example.com` will be treated as a MongoDB host and thus a MongoDB connection will be attempted.

If this connection is successful, a callback method is called where you can deal with the results. 
Sentinel by default stores them in a locally hosted MongoDB instance. Authentication is disabled, very nice.

Every service that requires a specific connection type will need its own scanner. 
An abstract class is provided for these scanners. Custom scanners can be implemented by extending this base class and registering them in `ScannerProvider`. 
If no custom scanners are required, `ScannerProvider.DefaultProvider` will suffice.

## Why?
My BSc. thesis was about information leakage through certificate transparency. 
During my research I found that a considerable percentage of services that announce their presence through domain name labels do not have authentication enabled or allow guest access.

My thesis used an older dataset, specifically the Google Argon 2021 dataset. 
Because this is a relatively old dataset, a lot of the domains listed no longer exist or the owners had time to fix their mistakes. 
This got me curious what differences could be observed when using more recent, near realtime CT logs. 