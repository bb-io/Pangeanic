using Apps.Pangeanic.Api;
using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Responses.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Pangeanic.Invocables;

public class AppInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected PangeanicClient Client { get; }
    
    public AppInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }
    
    protected async Task<EngineResponse> GetEngine(string engineId)
    {
        string endpoint = ApiEndpoints.Engines;
        var response = await Client.ExecuteRequestAsync<GetEnginesResponse>(endpoint,
            Method.Post, new BaseJsonRequest(), Creds);

        return response.Engines.FirstOrDefault(x => x.Id.ToString() == engineId) ??
               throw new Exception("Engine not found");
    }
}