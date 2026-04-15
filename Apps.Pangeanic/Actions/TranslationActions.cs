using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Requests.Blueprints;
using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Models.Responses.Api;
using Apps.Pangeanic.Models.Responses.Blueprints;
using Apps.Pangeanic.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff1;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList("Translation")]
public class TranslationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
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

        var engine = await ResolveEngine(input.TargetLanguage, input.SourceLanguage, input.EngineId);
        ValidateEngineCompatibility(engine, input.SourceLanguage, input.TargetLanguage);

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

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate a file by using Blackbird interoperability or Pangeanic native translation")]
    public async Task<TranslateFileResponse> TranslateFile([ActionParameter] TranslateFileRequest input)
    {
        if (input.File == null)
            throw new PluginMisconfigurationException("The file can not be empty.");

        if (string.IsNullOrWhiteSpace(input.TargetLanguage))
            throw new PluginMisconfigurationException("The target language can not be empty.");

        var strategy = ResolveFileTranslationStrategy(input.FileTranslationStrategy);

        if (strategy == "native")
            return await TranslateFileWithNativeStrategy(input);

        return await TranslateFileWithBlackbirdStrategy(input);
    }

    private async Task<TranslateFileResponse> TranslateFileWithNativeStrategy(TranslateFileRequest input)
    {
        if (!string.IsNullOrWhiteSpace(input.OutputFileHandling) &&
            !input.OutputFileHandling.Equals("original", StringComparison.OrdinalIgnoreCase))
            throw new PluginMisconfigurationException(
                "Pangeanic native translation currently supports only 'original' output file handling.");

        var engine = await ResolveEngine(input.TargetLanguage, input.SourceLanguage, input.EngineId);
        ValidateEngineCompatibility(engine, input.SourceLanguage, input.TargetLanguage);

        var fileId = await SendFileForTranslation(input, engine);
        var response = await PollFileTranslation(fileId);

        if (!response.Success)
            throw new PluginApplicationException(
                $"Failed to download translated file. Status: {response.Status}, Error: {response.Error?.Message}");

        if (response.Data == null || string.IsNullOrWhiteSpace(response.Data.File))
            throw new PluginApplicationException("Translated file data was not returned by the API.");

        var fileBytes = Convert.FromBase64String(response.Data.File);
        await using var memoryStream = new MemoryStream(fileBytes);

        var outputFile = await fileManagementClient.UploadAsync(
            memoryStream,
            MimeTypes.GetMimeType(response.Data.FileName),
            response.Data.FileName);

        return new TranslateFileResponse
        {
            File = outputFile
        };
    }

    private async Task<TranslateFileResponse> TranslateFileWithBlackbirdStrategy(TranslateFileRequest input)
    {
        try
        {
            await using var stream = await fileManagementClient.DownloadAsync(input.File);
            var content = await Transformation.Parse(stream, input.File.Name);

            return await HandleInteroperableTransformation(content, input);
        }
        catch (Exception e) when (e.Message.Contains("not supported", StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginMisconfigurationException(
                "The file format is not supported by the Blackbird interoperable strategy.");
        }
    }

    private async Task<TranslateFileResponse> HandleInteroperableTransformation(
        Transformation content,
        TranslateFileRequest input)
    {
        if (!string.IsNullOrWhiteSpace(input.SourceLanguage))
            content.SourceLanguage = input.SourceLanguage;

        if (!string.IsNullOrWhiteSpace(input.TargetLanguage))
            content.TargetLanguage = input.TargetLanguage;

        if (string.IsNullOrWhiteSpace(content.SourceLanguage) || string.IsNullOrWhiteSpace(content.TargetLanguage))
            throw new PluginMisconfigurationException("Source or target language not defined.");

        bool SegmentFilter(Segment segment)
        {
            if (string.IsNullOrWhiteSpace(LineElementMapper.RenderLine(segment.Source)))
                return false;

            return segment.State == null || segment.State == SegmentState.Initial;
        }

        var units = content.GetUnits()
            .Where(x => x?.Name != null)
            .ToList();

        if (!units.SelectMany(x => x.Segments).Any(SegmentFilter))
            return await BuildFileResponseByFormat(content, input);

        var processed = await units
            .Batch(batchSize: 25, segmentFilter: SegmentFilter)
            .Process<string>(async batch =>
            {
                var sourceTexts = batch
                    .Select(x => LineElementMapper.RenderLine(x.Segment.Source))
                    .ToList();

                var translatedTexts = await TranslateTextsBatch(
                    sourceTexts,
                    content.SourceLanguage!,
                    content.TargetLanguage!,
                    input.EngineId);

                if (translatedTexts.Count != sourceTexts.Count)
                {
                    translatedTexts = translatedTexts
                        .Take(sourceTexts.Count)
                        .Concat(Enumerable.Repeat(string.Empty, Math.Max(0, sourceTexts.Count - translatedTexts.Count)))
                        .ToList();
                }

                return translatedTexts;
            });

        foreach (var item in processed)
        {
            foreach (var result in item.Results)
            {
                if (string.IsNullOrWhiteSpace(result.Result))
                    continue;

                result.Segment.Target = LineElementMapper.MakeLine(result.Result);
                result.Segment.State = SegmentState.Translated;
            }
        }

        return await BuildFileResponseByFormat(content, input);
    }

    private async Task<List<string>> TranslateTextsBatch(
        List<string> texts,
        string sourceLanguage,
        string targetLanguage,
        string? engineId)
    {
        if (!texts.Any())
            return [];

        var engine = await ResolveEngine(targetLanguage, sourceLanguage, engineId);
        ValidateEngineCompatibility(engine, sourceLanguage, targetLanguage);

        var response = await Client.ExecuteRequestAsync<List<List<TranslationPairResponse>>>(
            ApiEndpoints.Translate,
            Method.Post,
            new ProcessTextApiRequest
            {
                Src = sourceLanguage,
                Tgt = targetLanguage,
                Engine = engine.Id,
                Text = texts.ToArray(),
                RunParams = new RunParamsModel
                {
                    Tags = []
                }
            },
            Creds);

        return response
            .Select(x => x.FirstOrDefault()?.Target ?? string.Empty)
            .ToList();
    }

    private async Task<TranslateFileResponse> BuildFileResponseByFormat(
        Transformation content,
        TranslateFileRequest input)
    {
        if (input.OutputFileHandling?.Equals("original", StringComparison.OrdinalIgnoreCase) == true)
        {
            try
            {
                var targetContent = content.Target();
                var outputFile = await fileManagementClient.UploadAsync(
                    targetContent.Serialize().ToStream(),
                    targetContent.OriginalMediaType ?? "application/octet-stream",
                    targetContent.OriginalName ?? input.File.Name);

                return new TranslateFileResponse
                {
                    File = outputFile
                };
            }
            catch
            {
                var xliffFallback = await fileManagementClient.UploadAsync(
                    content.Serialize().ToStream(),
                    MediaTypes.Xliff,
                    content.XliffFileName);

                return new TranslateFileResponse
                {
                    File = xliffFallback
                };
            }
        }

        if (input.OutputFileHandling?.Equals("xliff1", StringComparison.OrdinalIgnoreCase) == true)
        {
            var xliff1String = Xliff1Serializer.Serialize(content);
            var outputFile = await fileManagementClient.UploadAsync(
                xliff1String.ToStream(),
                MediaTypes.Xliff,
                content.XliffFileName);

            return new TranslateFileResponse
            {
                File = outputFile
            };
        }

        var xliffFile = await fileManagementClient.UploadAsync(
            content.Serialize().ToStream(),
            MediaTypes.Xliff,
            content.XliffFileName);

        return new TranslateFileResponse
        {
            File = xliffFile
        };
    }

    private async Task<EngineResponse> ResolveEngine(string targetLanguage, string? sourceLanguage, string? engineId)
    {
        if (!string.IsNullOrWhiteSpace(engineId) &&
            !string.IsNullOrWhiteSpace(sourceLanguage) &&
            !string.IsNullOrWhiteSpace(targetLanguage))
        {
            return new EngineResponse
            {
                Id = engineId,
                Src = sourceLanguage,
                Tgt = targetLanguage,
                Status = "0",
                Descr = engineId,
                Domain = string.Empty,
                Flavor = string.Empty,
                InServiceId = string.Empty,
                MtPostEditionProfileId = string.Empty,
                OffDomainMode = string.Empty,
                ProcessId = string.Empty,
                ServiceId = string.Empty,
                SetupParms = new SetupParmsResponse(),
                RunParms = new RunParmsResponse()
            };
        }

        if (!string.IsNullOrWhiteSpace(engineId))
            return await GetEngine(engineId);

        var engines = await GetEngines();
        var candidates = engines
            .Where(x => x.Tgt.Equals(targetLanguage, StringComparison.OrdinalIgnoreCase))
            .Where(x => string.IsNullOrWhiteSpace(sourceLanguage) ||
                        x.Src.Equals(sourceLanguage, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!candidates.Any())
        {
            throw new PluginMisconfigurationException(
                $"No engine found for target language '{targetLanguage}'" +
                (string.IsNullOrWhiteSpace(sourceLanguage)
                    ? "."
                    : $" and source language '{sourceLanguage}'."));
        }

        var activeCandidates = candidates
            .Where(x => x.Status.Equals("active", StringComparison.OrdinalIgnoreCase) ||
                        x.Status.Equals("0", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (activeCandidates.Count == 1)
            return activeCandidates[0];

        if (candidates.Count == 1)
            return candidates[0];

        throw new PluginMisconfigurationException(
            "Multiple engines match the requested languages. Specify 'Engine ID' to choose the correct engine.");
    }

    private static void ValidateEngineCompatibility(EngineResponse engine, string? sourceLanguage, string targetLanguage)
    {
        if (!string.IsNullOrWhiteSpace(sourceLanguage) &&
            !engine.Src.Equals(sourceLanguage, StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginMisconfigurationException(
                $"Selected engine supports source language '{engine.Src}', but '{sourceLanguage}' was requested.");
        }

        if (!string.IsNullOrWhiteSpace(targetLanguage) &&
            !engine.Tgt.Equals(targetLanguage, StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginMisconfigurationException(
                $"Selected engine supports target language '{engine.Tgt}', but '{targetLanguage}' was requested.");
        }
    }

    private async Task<string> SendFileForTranslation(TranslateFileRequest input, EngineResponse engine)
    {
        using var content = new MultipartFormDataContent("----WebKitFormBoundary8M3sSU13ul5lXSJm");

        var fileName = input.FileName ?? input.File.Name;
        content.Add(new StringContent(fileName), "title");
        content.Add(new StringContent(engine.Id), "engine");
        content.Add(new StringContent(engine.Src), "src");
        content.Add(new StringContent(engine.Tgt), "tgt");
        content.Add(new StringContent(Creds.GetToken()), "apikey");
        content.Add(new StringContent("translation"), "processname");

        if (!string.IsNullOrWhiteSpace(input.ProcessOption))
            content.Add(new StringContent(input.ProcessOption), "processoption");

        content.Add(new StringContent(input.Username ?? "blackbird"), "username");

        await using var fileStream = await fileManagementClient.DownloadAsync(input.File);
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);

        var fileContent = new ByteArrayContent(memoryStream.ToArray());
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(Creds.GetUrl() + ApiEndpoints.SendFile, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new PluginApplicationException(
                $"Failed to upload file. Status code: {response.StatusCode}, Content: {responseContent}");
        }

        var processResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessFileResponse>(responseContent);
        if (processResponse == null || string.IsNullOrWhiteSpace(processResponse.FileId))
            throw new PluginApplicationException("Pangeanic did not return a file ID.");

        return processResponse.FileId;
    }

    private async Task<RetrieveFileResponse> PollFileTranslation(string fileId)
    {
        string? lastResponseContent = null;

        for (var i = 0; i < 60; i++)
        {
            var request = new RestRequest(Creds.GetUrl() + ApiEndpoints.CheckFile, Method.Get);
            request.AddQueryParameter("apikey", Creds.GetToken());
            request.AddQueryParameter("guid", fileId);

            var response = await Client.ExecuteRequest(request);
            lastResponseContent = response.Content;
            var statusResponse = ParseCheckFileResponse(response.Content);

            if (statusResponse == null || (statusResponse.EffectiveStatus == null &&
                                           string.IsNullOrWhiteSpace(statusResponse.EffectiveMessage)))
            {
                var emptyStatusDownload = await TryRetrieveTranslatedFile(fileId, suppressExceptions: true);
                if (emptyStatusDownload != null)
                    return emptyStatusDownload;

                await Task.Delay(TimeSpan.FromSeconds(2));
                continue;
            }

            var message = statusResponse.EffectiveMessage;
            var status = statusResponse.EffectiveStatus;

            if (status == 100 || string.Equals(message, "Finished", StringComparison.OrdinalIgnoreCase))
            {
                var completedDownload = await TryRetrieveTranslatedFile(fileId, suppressExceptions: false);
                if (completedDownload != null)
                    return completedDownload;
            }

            if (!string.IsNullOrWhiteSpace(message) &&
                (message.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                 message.Contains("fail", StringComparison.OrdinalIgnoreCase)))
            {
                throw new PluginApplicationException($"File translation failed: {message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new PluginApplicationException(
            $"Timed out while waiting for file translation to finish. Last status response: {lastResponseContent}");
    }

    private static CheckFileResponse? ParseCheckFileResponse(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        var token = JToken.Parse(content);

        if (token.Type == JTokenType.Array)
        {
            var firstItem = token.FirstOrDefault();
            return firstItem == null ? null : ParseCheckFileResponse(firstItem.ToString());
        }

        if (token.Type == JTokenType.Object)
        {
            var dataToken = token["data"]?.Type == JTokenType.Object ? token["data"] : null;

            return new CheckFileResponse
            {
                FileId = token.Value<string>("fileid"),
                Status = ReadStatusValue(dataToken?["status"] ?? token["status"]),
                Message = ReadMessageValue(dataToken?["message"] ?? token["message"])
            };
        }

        if (token.Type == JTokenType.String)
        {
            return new CheckFileResponse
            {
                Message = token.ToString()
            };
        }

        if (token.Type == JTokenType.Integer)
        {
            return new CheckFileResponse
            {
                Status = token.Value<int>()
            };
        }

        return null;
    }

    private static int? ReadStatusValue(JToken? token)
    {
        if (token == null || token.Type is JTokenType.Null or JTokenType.Undefined)
            return null;

        if (token.Type == JTokenType.Integer)
            return token.Value<int>();

        if (token.Type == JTokenType.String &&
            int.TryParse(token.Value<string>(), out var parsedStatus))
        {
            return parsedStatus;
        }

        return null;
    }

    private static string? ReadMessageValue(JToken? token)
    {
        if (token == null || token.Type is JTokenType.Null or JTokenType.Undefined)
            return null;

        return token.Type == JTokenType.String ? token.Value<string>() : token.ToString();
    }

    private async Task<RetrieveFileResponse?> TryRetrieveTranslatedFile(string fileId, bool suppressExceptions)
    {
        try
        {
            var apiRequest = new RetrieveFileRequest(new Models.Requests.DownloadFileRequest
            {
                FileId = fileId
            })
            {
                ApiKey = Creds.GetToken()
            };

            var response = await Client.ExecuteRequestAsync<RetrieveFileResponse>(
                ApiEndpoints.DownloadFile,
                Method.Post,
                apiRequest,
                Creds);

            if (!response.Success || response.Data == null || string.IsNullOrWhiteSpace(response.Data.File))
                return null;

            return response;
        }
        catch (Exception) when (suppressExceptions)
        {
            return null;
        }
    }

    private static string ResolveFileTranslationStrategy(string? strategy)
    {
        if (string.IsNullOrWhiteSpace(strategy))
            return "blackbird";

        return strategy.Equals("native", StringComparison.OrdinalIgnoreCase)
            ? "native"
            : strategy.Equals("blackbird", StringComparison.OrdinalIgnoreCase)
                ? "blackbird"
                : throw new PluginMisconfigurationException("Unsupported file translation strategy.");
    }
}
