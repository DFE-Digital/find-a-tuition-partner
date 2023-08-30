
#### Azure Blob Storage Configs For Enquiries Data

The Enquiries data is extracted from the FaTP database to a container in Azure Blob Storage.

**You must get a client secret and run the four `dotnet user-secrets set` commands below for the data extraction function to work. To request the client secret, please contact the project lead.**

You can then use the following commands to add the necessary user secrets after obtaining the client secret:

```
dotnet user-secrets set "BlobStorageEnquiriesData:ClientSecret" "<will be shared privately>" -p Functions
dotnet user-secrets set "BlobStorageEnquiriesData:ContainerName" "ntp-enquiries-data-qa" -p Functions
dotnet user-secrets set "BlobStorageEnquiriesData:AccountName" "s177p01enquiriesdata" -p Functions
dotnet user-secrets set "BlobStorageEnquiriesData:ClientId" "10e39e46-b948-4cd9-ac27-90dc6e4bd472" -p Functions
dotnet user-secrets set "BlobStorageEnquiriesData:TenantId" "9c7d9dd3-840c-4b3f-818e-552865082e16" -p Functions
```

#### ```DataExtraction``` Function 

You can configure the value of the ```DataExtraction``` timer function by setting the value for the following example value set to trigger the timer function once every five minutes:
```
dotnet user-secrets set "DataExtractionTimerCronExpression" "0 */5 * * * *" -p Functions
```

