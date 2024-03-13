using Blackbird.Applications.Sdk.Common;

namespace Apps.Pangeanic.Models.Requests;

public class DownloadFileRequest
{
    [Display("File ID")]
    public string FileId { get; set; }
}