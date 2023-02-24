namespace ApiBureau.Bullhorn.Api;

//https://github.com/microsoftgraph/msgraph-sdk-dotnet-core/blob/feature/3.0/src/Microsoft.Graph.Core/Requests/BaseClient.cs
public class BaseClient
{
    public string BaseUrl { get; set; }

    public BaseClient(string baseUrl) => BaseUrl = baseUrl;
}