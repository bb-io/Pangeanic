using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Models.Responses.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Pangeanic.DataSourceHandlers;

public class EngineDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        string endpoint = ApiEndpoints.Engines;
        var response = await Client.ExecuteRequestAsync<GetEnginesResponse>(endpoint,
            Method.Post, new BaseJsonRequest(), Creds);

        return response.Engines
            .Where(x => context.SearchString == null ||
                        x.Descr.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToDictionary(x => x.Id.ToString(), BuildReadableName);
    }
    
    private string BuildReadableName(EngineResponse engine)
    {
        return $"{engine.Descr} - {engine.Tgt}";
    }
}