using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class SetupParmsResponse
{
    [Display("Always started"), JsonProperty("always-started")]
    public bool AlwaysStarted { get; set; }
    
    [Display("Min nodes"), JsonProperty("minnodes")]
    public int MinNodes { get; set; }
}