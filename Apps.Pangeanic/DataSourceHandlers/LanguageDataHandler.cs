using Apps.Pangeanic.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Pangeanic.DataSourceHandlers;

public class LanguageDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<string> languages = (await GetEngines())
            .SelectMany(x => new[]
            {
                x.Src,
                x.Tgt
            })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x);

        if (!string.IsNullOrWhiteSpace(context.SearchString))
        {
            languages = languages.Where(x =>
                x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));
        }

        return languages.Select(x => new DataSourceItem(x, x));
    }
}
