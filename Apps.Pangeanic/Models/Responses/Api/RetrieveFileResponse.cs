using Apps.Pangeanic.Utils;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class RetrieveFileResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("error"), JsonConverter(typeof(FileErrorResponseConverter))]
    public FileErrorResponse Error { get; set; }
    
    [JsonProperty("data"), JsonConverter(typeof(FileResponseConverter))]
    public FileResponse Data { get; set; }
}