using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Pangeanic.DataSourceHandlers.EnumHandlers;

public class ProcessNameHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "translation", "Translation" }
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}