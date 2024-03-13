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
}