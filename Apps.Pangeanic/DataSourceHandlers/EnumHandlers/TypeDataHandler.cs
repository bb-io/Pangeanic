using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Pangeanic.DataSourceHandlers.EnumHandlers;

public class TypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>()
        {
            { "Blacklining", "Blacklining"},
            { "TAG:idx", "TAG:idx"}
        };
    }
}