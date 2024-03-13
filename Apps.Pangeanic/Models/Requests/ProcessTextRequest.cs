using Blackbird.Applications.Sdk.Common;

namespace Apps.Pangeanic.Models.Requests;

public class ProcessTextRequest : ProcessTextRequestBase
{
    [Display("Text")]
    public IEnumerable<string> Text { get; set; }
}