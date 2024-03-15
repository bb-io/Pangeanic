using Apps.Pangeanic.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Pangeanic.Models.Requests;

public abstract class ProcessTextRequestBase
{
    [Display("Engine ID"), DataSource(typeof(EngineDataHandler))]
    public string EngineId { get; set; }

    [Display("Glossary ID")]
    public string? GlossaryId { get; set; }

    [Display("Sensitivity", Description = "Should be a value between 0 and 1. The higher the value, the more sensitive the engine will be")]
    public string? Sensitivity { get; set; }

    public string? Type { get; set; }

    public IEnumerable<string>? Tags { get; set; }
}