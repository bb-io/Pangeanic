using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Requests.Blueprints;
using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Models.Responses.Api;
using Apps.Pangeanic.Models.Responses.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList("Translation")]
public class TranslationActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [BlueprintActionDefinition(BlueprintAction.TranslateText)]
    [Action("Translate text", Description = "Translate a single text string")]
    public async Task<TranslateTextResponse> TranslateText(
        [ActionParameter] TranslateTextRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.Text))
            throw new PluginMisconfigurationException("The text can not be empty.");

        if (string.IsNullOrWhiteSpace(input.TargetLanguage))
            throw new PluginMisconfigurationException("The target language can not be empty.");

        var engine = await ResolveEngine(input);
        ValidateEngineCompatibility(engine, input);

        var sourceLanguage = input.SourceLanguage ?? engine.Src;
        var targetLanguage = input.TargetLanguage ?? engine.Tgt;

        var apiRequest = new ProcessTextApiRequest
        {
            Src = sourceLanguage,
            Tgt = targetLanguage,
            Engine = engine.Id,
            GlossaryId = string.IsNullOrWhiteSpace(input.GlossaryId) ? null : int.Parse(input.GlossaryId),
            Text = [input.Text],
            RunParams = new RunParamsModel
            {
                Sensitivity = input.Sensitivity,
                Type = input.Type,
                Tags = input.Tags?.ToList()
            }
        };

        var response = await Client.ExecuteRequestAsync<List<List<TranslationPairResponse>>>(
            ApiEndpoints.Translate,
            Method.Post,
            apiRequest,
            Creds);

        var translation = response.FirstOrDefault()?.FirstOrDefault();
        if (translation == null)
            throw new PluginApplicationException("No translation found.");

        return new TranslateTextResponse
        {
            TranslatedText = translation.Target,
            DetectedSourceLanguage = null
        };
    }

    private async Task<EngineResponse> ResolveEngine(TranslateTextRequest input)
    {
        if (!string.IsNullOrWhiteSpace(input.EngineId))
            return await GetEngine(input.EngineId);

        var engines = await GetEngines();
        var candidates = engines
            .Where(x => x.Tgt.Equals(input.TargetLanguage, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrWhiteSpace(input.SourceLanguage) ||
                        x.Src.Equals(input.SourceLanguage, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!candidates.Any())
            throw new PluginMisconfigurationException(
                $"No engine found for target language '{input.TargetLanguage}'" +
                (string.IsNullOrWhiteSpace(input.SourceLanguage)
                    ? "."
                    : $" and source language '{input.SourceLanguage}'."));

        var activeCandidates = candidates
            .Where(x => x.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (activeCandidates.Count == 1)
            return activeCandidates[0];

        if (candidates.Count == 1)
            return candidates[0];

        throw new PluginMisconfigurationException(
            "Multiple engines match the requested languages. Specify 'Engine ID' to choose the correct engine.");
    }

    private static void ValidateEngineCompatibility(EngineResponse engine, TranslateTextRequest input)
    {
        if (!string.IsNullOrWhiteSpace(input.SourceLanguage) &&
            !engine.Src.Equals(input.SourceLanguage, StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginMisconfigurationException(
                $"Selected engine supports source language '{engine.Src}', but '{input.SourceLanguage}' was requested.");
        }

        if (!string.IsNullOrWhiteSpace(input.TargetLanguage) &&
            !engine.Tgt.Equals(input.TargetLanguage, StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginMisconfigurationException(
                $"Selected engine supports target language '{engine.Tgt}', but '{input.TargetLanguage}' was requested.");
        }
    }
}
