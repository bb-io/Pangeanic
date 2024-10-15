using Blackbird.Applications.Sdk.Common;

namespace Apps.Pangeanic.Models.Requests;

public class FileOptionalRequest
{
    [Display("File ID")]
    public string? FileId { get; set; }
}