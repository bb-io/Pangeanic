using Blackbird.Applications.Sdk.Common;

namespace Apps.Pangeanic.Models.Responses;

public class ProcessTextAsStringResponse
{
    [Display("Source text")]
    public string Source { get; set; }
    
    [Display("Target text")]
    public string Target { get; set; }
}