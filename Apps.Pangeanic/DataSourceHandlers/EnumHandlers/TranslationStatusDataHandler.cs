﻿using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Pangeanic.DataSourceHandlers.EnumHandlers;

public class TranslationStatusDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "Preprocessing", "Preprocessing" },
        { "Processing", "Processing" },
        { "PostProcessing", "Postprocessing" },
        { "Finished", "Finished" },
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}