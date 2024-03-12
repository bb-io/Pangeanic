namespace Apps.Pangeanic.Models.Requests.Api;

public class RetrieveFileRequest : BaseJsonRequest
{
    public string FileId { get; set; }

    public RetrieveFileRequest(DownloadFileRequest request)
    {
        FileId = request.FileId;
    }
}