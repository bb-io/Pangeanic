using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Requests.Api;

public class RetrieveFileRequest : BaseJsonRequest
{
    [JsonProperty("guid")]
    public string FileId { get; set; }

    public RetrieveFileRequest(DownloadFileRequest request)
    {
        FileId = request.FileId;
    }
}