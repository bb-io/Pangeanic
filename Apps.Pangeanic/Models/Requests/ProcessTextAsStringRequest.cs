using Blackbird.Applications.Sdk.Common;

namespace Apps.Pangeanic.Models.Requests;

public class ProcessTextAsStringRequest : ProcessTextRequestBase
{
    [Display("Text")]
    public string Text { get; set; }
}