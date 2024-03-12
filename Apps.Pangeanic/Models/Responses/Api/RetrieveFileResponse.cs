using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class RetrieveFileResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("error")]
    public FileErrorResponse Error { get; set; }
    
    [JsonProperty("data")]
    public FileResponse Data { get; set; }
}