using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList]
public class EngineActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Get engines", Description = "Get engines from api")]
    public async Task<GetEnginesResponse> GetEngines()
    {
        return await Client.ExecuteRequestAsync<GetEnginesResponse>(ApiEndpoints.Corp + ApiEndpoints.Engines,
            Method.Post, new BaseJsonRequest(), Creds);
    }
}