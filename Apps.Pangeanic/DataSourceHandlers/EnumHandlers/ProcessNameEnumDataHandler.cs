using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Pangeanic.DataSourceHandlers.EnumHandlers;

public class ProcessNameEnumDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "translation", "Translation" }
    };
}