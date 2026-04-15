using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class CheckFileResponse
{
    [JsonProperty("fileid")]
    public string? FileId { get; set; }

    [JsonProperty("data")]
    public CheckFileDataResponse? Data { get; set; }

    [JsonProperty("status")]
    public int? Status { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonIgnore]
    public int? EffectiveStatus => Data?.Status ?? Status;

    [JsonIgnore]
    public string? EffectiveMessage => Data?.Message ?? Message;
}
