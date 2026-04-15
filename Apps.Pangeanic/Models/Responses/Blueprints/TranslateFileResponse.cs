using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Pangeanic.Models.Responses.Blueprints;

public class TranslateFileResponse : ITranslateFileOutput
{
    [Display("File")]
    public FileReference File { get; set; } = default!;
}
