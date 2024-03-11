using Apps.Pangeanic.Utils;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class EngineResponse
{
    [Display("Engine ID")]
    public string Id { get; set; }
    
    [Display("Process ID")]
    public string ProcessId { get; set; }
    
    [Display("Service ID")]
    public string ServiceId { get; set; }
    
    [Display("In Service ID")]
    public string InServiceId { get; set; }
    
    [Display("Source language")]
    public string Src { get; set; }
    
    [Display("Target language")]
    public string Tgt { get; set; }
    
    [Display("Description")]
    public string Descr { get; set; }
    
    [Display("Domain")]
    public string Domain { get; set; }
    
    [Display("Flavor")]
    public string Flavor { get; set; }
    
    [Display("Status")]
    public string Status { get; set; }
    
    [Display("Creation date")]
    public string OffDomainMode { get; set; }
    
    [Display("Setup parameters"), JsonConverter(typeof(SetupParmsConverter))]
    public SetupParmsResponse SetupParms { get; set; }
    
    [Display("Run parameters"), JsonProperty("runparms"), JsonConverter(typeof(RunParmsConverter))]
    public RunParmsResponse RunParms { get; set; }
    
    [Display("Post-edition profile ID")]
    public string MtPostEditionProfileId { get; set; }
}