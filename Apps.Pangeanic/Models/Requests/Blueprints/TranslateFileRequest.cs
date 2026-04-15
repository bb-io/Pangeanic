using Apps.Pangeanic.DataSourceHandlers;
using Apps.Pangeanic.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Handlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Pangeanic.Models.Requests.Blueprints;

public class TranslateFileRequest : ITranslateFileInput
{
    [Display("File")]
    public FileReference File { get; set; } = default!;

    [Display("Target language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Source language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguage { get; set; }

    [Display("Engine ID")]
    [DataSource(typeof(EngineDataHandler))]
    public string? EngineId { get; set; }

    [Display("Username")]
    public string? Username { get; set; }

    [Display("Process option")]
    public string? ProcessOption { get; set; }

    [Display("File name")]
    public string? FileName { get; set; }

    [Display("File translation strategy", Description = "Optional. Defaults to Blackbird when empty.")]
    [StaticDataSource(typeof(FileTranslationStrategyHandler))]
    public string? FileTranslationStrategy { get; set; }

    [Display("Output file handling", Description = "original = return original format when possible; otherwise returns XLIFF")]
    [StaticDataSource(typeof(ProcessFileFormatHandler))]
    public string? OutputFileHandling { get; set; }
}
