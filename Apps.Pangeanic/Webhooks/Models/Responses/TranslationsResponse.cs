using Blackbird.Applications.Sdk.Common;

namespace Apps.Pangeanic.Webhooks.Models.Responses;

public class TranslationsResponse
{
    [Display("File ID")]
    public string FileId { get; set; }

    [Display("Translation status")]
    public string Status { get; set; }
}