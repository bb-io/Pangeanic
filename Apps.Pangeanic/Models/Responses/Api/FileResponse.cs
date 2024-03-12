using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class FileResponse
{
    [JsonProperty("guid")]
    public string Guid { get; set; }

    [JsonProperty("fileType")]
    public string FileType { get; set; }
    
    [JsonProperty("filename")]
    public string FileName { get; set; }

    [JsonProperty("file")]
    public string File { get; set; }
}