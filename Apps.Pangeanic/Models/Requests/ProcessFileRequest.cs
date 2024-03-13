using Apps.Pangeanic.DataSourceHandlers;
using Apps.Pangeanic.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Pangeanic.Models.Requests;

public class ProcessFileRequest
{
    [Display("Engine"), DataSource(typeof(EngineDataHandler))]
    public string EngineId { get; set; }
    
    public FileReference File { get; set; }
    
    [Display("Process name"), DataSource(typeof(ProcessNameEnumDataHandler))]
    public string ProcessName { get; set; }

    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }

    public string? Username { get; set; }

    [Display("Process option")]
    public string? ProcessOption { get; set; }
    
    [Display("File name")]
    public string? FileName { get; set; }
}