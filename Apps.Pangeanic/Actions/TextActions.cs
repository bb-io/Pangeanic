using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Models.Responses.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList]
public class TextActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Process text", Description = "Get engines from api")]
    public async Task<ProcessTextResponse> TranslateText([ActionParameter]ProcessTextRequest request)
    {
        var response = await Client.ExecuteRequestAsync<ProcessTextApiResponse>(ApiEndpoints.Translate,
            Method.Post, new ProcessTextApiRequest(request), Creds);
        
        var processedText = new ProcessTextResponse()
        {
            Translations = response.TranslationPairs.Select(x => x.FirstOrDefault()).ToList()
        };
        
        return processedText;
    }
}