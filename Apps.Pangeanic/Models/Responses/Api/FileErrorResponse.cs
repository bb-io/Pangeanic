using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class FileErrorResponse
{
    [JsonProperty("statusCode")]
    public string StatusCode { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }

    public override string ToString()
    {
        return $"Status code: {StatusCode}; Code: {Code}; Message: {Message}";
    }
}