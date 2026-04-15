using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Pangeanic.DataSourceHandlers.EnumHandlers;

public class FileTranslationStrategyHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "blackbird", "Blackbird (interoperable translation workflows)" },
        { "native", "Pangeanic (native document translation)" }
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}
