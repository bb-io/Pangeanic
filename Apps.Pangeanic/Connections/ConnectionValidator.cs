using Apps.Pangeanic.Api;
using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Pangeanic.Connections;

public class ConnectionValidator: IConnectionValidator
{
    private PangeanicClient _client = new();
    
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            await _client.ExecuteRequestAsync<GetEnginesResponse>(ApiEndpoints.Corp + ApiEndpoints.Engines, Method.Post, new BaseJsonRequest(), authenticationCredentialsProviders.ToArray());
            return new() { IsValid = true };
        }
        catch (Exception e)
        {
            return new ConnectionValidationResponse { IsValid = false, Message = e.Message };
        }
    }
}