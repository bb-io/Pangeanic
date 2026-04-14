using Apps.Pangeanic.DataSourceHandlers;
using Apps.Pangeanic.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Pangeanic.Models.Requests.Blueprints;

public class TranslateTextRequest : ITranslateTextInput
{
    [Display("Text")]
    public string Text { get; set; } = string.Empty;

    [Display("Target language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Source language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Engine ID")]
    [DataSource(typeof(EngineDataHandler))]
    public string? EngineId { get; set; }

    [Display("Glossary ID")]
    public string? GlossaryId { get; set; }

    [Display("Sensitivity", Description = "Should be a value between 0 and 1. The higher the value, the more sensitive the engine will be")]
    public string? Sensitivity { get; set; }

    [StaticDataSource(typeof(TypeDataHandler))]
    public string? Type { get; set; }

    public IEnumerable<string>? Tags { get; set; }
}
