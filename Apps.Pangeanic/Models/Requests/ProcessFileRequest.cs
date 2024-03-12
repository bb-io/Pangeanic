using Apps.Pangeanic.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Pangeanic.Models.Requests;

public class ProcessFileRequest
{
    [Display("Engine"), DataSource(typeof(EngineDataHandler))]
    public string EngineId { get; set; }
    
    [Display("Source language")]
    public string SourceLanguage { get; set; }
    
    [Display("Target language")]
    public string TargetLanguage { get; set; }
    
    public FileReference File { get; set; }

    [Display("Callback URL")]
    public string? CallbackUrl { get; set; }

    public string? Username { get; set; }

    [Display("Process option")]
    public string? ProcessOption { get; set; }

    [Display("Process name")]
    public string? ProcessName { get; set; } = "translation";
    
    [Display("File name")]
    public string? FileName { get; set; }
}