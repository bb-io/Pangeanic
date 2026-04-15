using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Pangeanic.Models.Responses.Blueprints;

public class TranslateTextResponse : ITranslateTextOutput
{
    [Display("Translated text")]
    public string TranslatedText { get; set; } = string.Empty;

    [Display("Detected source language")]
    public string? DetectedSourceLanguage { get; set; }
}
