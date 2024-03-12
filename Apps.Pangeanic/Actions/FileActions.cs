using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Models.Responses.Api;
using Apps.Pangeanic.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Pangeanic.Actions;

[ActionList]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Process file", Description = "Upload file to be translated")]
    public async Task<ProcessFileResponse> ProcessFile([ActionParameter] ProcessFileRequest request)
    {
        var apikey = Creds.GetToken();
        string fileName = request.FileName ?? request.File.Name;

        using var content = new MultipartFormDataContent("----WebKitFormBoundary8M3sSU13ul5lXSJm");

        content.Add(new StringContent(fileName), "title");
        content.Add(new StringContent(request.EngineId), "engine");
        content.Add(new StringContent(request.SourceLanguage), "src");
        content.Add(new StringContent(request.TargetLanguage), "tgt");
        content.Add(new StringContent(apikey), "apikey");
        content.Add(new StringContent(request.ProcessName), "processname");
        if (request.CallbackUrl != null) content.Add(new StringContent(request.CallbackUrl), "notiflink");
        if (request.ProcessOption != null) content.Add(new StringContent(request.ProcessOption), "processoption");
        if (request.Username != null) content.Add(new StringContent(request.Username), "username");

        var fileStream = await fileManagementClient.DownloadAsync(request.File);
        var bytes = ReadFully(fileStream);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", fileName);

        var client = new HttpClient();
        var apiUrl = Creds.GetUrl() + ApiEndpoints.SendFile;
        var response = await client.PostAsync(apiUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Failed to upload file. Status code: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ProcessFileResponse>(responseContent);
    }

    [Action("Download file", Description = "Download translated file based on file ID")]
    public async Task<DownloadFileResponse> DownloadFile([ActionParameter] DownloadFileRequest request)
    {
        var apiRequest = new RetrieveFileRequest(request) { ApiKey = Creds.GetToken() };
        var response =
            await Client.ExecuteRequestAsync<RetrieveFileResponse>(ApiEndpoints.DownloadFile, Method.Post, apiRequest,
                Creds);

        if (!response.Success)
        {
            throw new Exception($"Failed to download file. Status: {response.Status}, Error: {response.Error}");
        }

        if (response.Data == null)
        {
            throw new Exception("Failed to download file. No data returned from api");
        }

        byte[] fileBytes = Convert.FromBase64String(response.Data.File);
        using var memoryStream = new MemoryStream(fileBytes);

        var contentType = MimeTypes.GetMimeType(response.Data.FileName);
        var fileReference = await fileManagementClient.UploadAsync(memoryStream, contentType, response.Data.FileName);

        return new DownloadFileResponse { File = fileReference };
    }

    private static byte[] ReadFully(Stream input)
    {
        using var ms = new MemoryStream();
        input.CopyTo(ms);

        return ms.ToArray();
    }
}