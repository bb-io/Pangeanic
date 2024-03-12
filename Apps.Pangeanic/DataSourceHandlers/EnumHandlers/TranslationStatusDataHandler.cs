using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Pangeanic.DataSourceHandlers.EnumHandlers;

public class TranslationStatusDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "Preprocessing", "Preprocessing" },
        { "Processing", "Processing" },
        { "PostProcessing", "Postprocessing" },
        { "Finished", "Finished" },
    };
}