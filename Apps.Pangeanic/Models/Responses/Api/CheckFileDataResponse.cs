using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class CheckFileDataResponse
{
    [JsonProperty("status")]
    public int? Status { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}
