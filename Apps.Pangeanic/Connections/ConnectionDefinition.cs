using Apps.Pangeanic.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Pangeanic.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    private static IEnumerable<ConnectionProperty> ConnectionProperties => new[]
    {
        new ConnectionProperty(CredsNames.Url)
        {
            DisplayName = "URL", Description = "URL of the service, for example: http://prod.pangeamt.com:8080/NexRelay/v1/", Sensitive = false
        },
        new ConnectionProperty(CredsNames.ApiKey)
        {
            DisplayName = "API Key", Description = "API key for the Pangeanic API", Sensitive = true
        }
    };
    
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = ConnectionProperties
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var urlKeyValue = values.First(v => v.Key == CredsNames.Url);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            urlKeyValue.Key,
            urlKeyValue.Value
        );
        
        var apiKeyKeyValue = values.First(v => v.Key == CredsNames.ApiKey);
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            apiKeyKeyValue.Key,
            apiKeyKeyValue.Value
        );
    }
}