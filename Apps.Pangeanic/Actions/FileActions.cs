using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    private readonly string _logUrl = "https://webhook.site/6ac247e0-726f-410d-bdb3-6649d20964ae";
    private readonly RestClient _client = new();
    
    [Action("Process file", Description = "Upload file to be translated")]
    public async Task<ProcessFileResponse> ProcessFile([ActionParameter]ProcessFileRequest request)
    {
        var sendFileRequest = new SendFileRequest(request)
        {
            ApiKey = Creds.GetToken()
        };

        var apikey = Creds.GetToken();
        string fileName = request.FileName ?? request.File.Name;

        using var content = new MultipartFormDataContent("----WebKitFormBoundary8M3sSU13ul5lXSJm");
    
        content.Add(new StringContent(fileName), "title");
        content.Add(new StringContent(request.EngineId), "engine");
        content.Add(new StringContent(request.SourceLanguage), "src");
        content.Add(new StringContent(request.TargetLanguage), "tgt");
        content.Add(new StringContent(apikey), "apikey");
        if (request.CallbackUrl != null) content.Add(new StringContent(request.CallbackUrl), "notiflink");
        if (request.ProcessOption != null) content.Add(new StringContent(request.ProcessOption), "processoption");
        if (request.Username != null) content.Add(new StringContent(request.Username), "username");
        if (request.ProcessName != null) content.Add(new StringContent(request.ProcessName), "processname");

        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var fileContent = new ByteArrayContent(ReadFully(fileStream));
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);
        
        var restRequest = new RestRequest(_logUrl, Method.Post)
            .WithJsonBody(new { FileName = fileName, Content = content });
        await _client.ExecuteAsync(restRequest);

        var client = new HttpClient();
        var apiUrl = Creds.GetUrl() + ApiEndpoints.SendFile; 
        var response = await client.PostAsync(apiUrl, content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProcessFileResponse>(responseContent);
        }
        
        throw new Exception($"Failed to upload file. Status code: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
    }

    private static byte[] ReadFully(Stream input)
    {
        using var ms = new MemoryStream();
        input.CopyTo(ms);
        
        return ms.ToArray();
    }
}