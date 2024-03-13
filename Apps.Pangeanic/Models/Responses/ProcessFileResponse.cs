using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses;

public class ProcessFileResponse
{
    [Display("File ID"), JsonProperty("fileId")]
    public string FileId { get; set; }
}