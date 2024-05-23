using Apps.Pangeanic.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Pangeanic.Webhooks.Payload;

public class TranslationStatusUpdatedInput
{
    [Display("Translation status"), StaticDataSource(typeof(TranslationStatusDataHandler))]
    public string? TranslationStatus { get; set; }
}