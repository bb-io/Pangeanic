using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Requests.Api;

public class SendFileRequest : BaseJsonRequest
{
    [JsonProperty("engine")]
    public string EngineId { get; set; }
    
    [JsonProperty("src")]
    public string SourceLanguage { get; set; }
    
    [JsonProperty("tgt")]
    public string TargetLanguage { get; set; }

    [JsonProperty("notiflink")]
    public string? CallbackUrl { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("processoption")]
    public string? ProcessOption { get; set; }

    [JsonProperty("processname")]
    public string? ProcessName { get; set; }
    
    [JsonProperty("title")]
    public string FileName { get; set; }

    public SendFileRequest(ProcessFileRequest request)
    {
        EngineId = request.EngineId;
        SourceLanguage = request.SourceLanguage;
        TargetLanguage = request.TargetLanguage;
        CallbackUrl = request.CallbackUrl;
        Username = request.Username ?? "pangeanic";
        ProcessOption = request.ProcessOption ?? "1";
        ProcessName = request.ProcessName ?? "translation";
        FileName = request.FileName ?? request.File.Name;
    }
}