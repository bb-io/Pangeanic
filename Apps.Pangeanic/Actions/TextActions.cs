using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList]
public class TextActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Process multiple texts", Description = "Send multiple texts to be processed and translated")]
    public async Task<ProcessTextResponse> TranslateText([ActionParameter] ProcessTextRequest request)
    {
        var engine = await GetEngine(request.EngineId);
        var apiRequest = new ProcessTextApiRequest(request, engine.Src, engine.Tgt);
        var response = await Client.ExecuteRequestAsync<List<List<TranslationPairResponse>>>(ApiEndpoints.Translate,
            Method.Post, apiRequest, Creds);

        var processedText = new ProcessTextResponse()
        {
            Translations = response.Select(x => x.FirstOrDefault()).ToList()
        };

        return processedText;
    }

    [Action("Process text", Description = "Send text to be processed and translated as a single string")]
    public async Task<TranslationPairResponse> TranslateTextAsSingleString(
        [ActionParameter] ProcessTextAsStringRequest request)
    {
        
        var engine = await GetEngine(request.EngineId);
        var apiRequest = new ProcessTextApiRequest(request, engine.Src, engine.Tgt);
        
        var response = await Client.ExecuteRequestAsync<List<List<TranslationPairResponse>>>(ApiEndpoints.Translate,
            Method.Post, apiRequest, Creds);

        var translation = response.FirstOrDefault()?.FirstOrDefault();
        if (translation == null)
        {
            throw new Exception("No translation found");
        }

        return translation;
    }
}