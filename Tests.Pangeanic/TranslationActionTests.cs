using Apps.Pangeanic.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using System.Reflection;
using Tests.Pangeanic.Base;

namespace Tests.Pangeanic;

[TestClass]
public class TranslationActionTests : TestBase
{
    [TestMethod]
    public async Task TranslateText_ShouldReturnTranslation()
    {
        var action = new TranslationActions(InvocationContext, FileManager);
        var input = new Apps.Pangeanic.Models.Requests.Blueprints.TranslateTextRequest
        {
            Text = "Hello, world!",
            TargetLanguage = "es",
            EngineId = "2154"
        };

        var result = await action.TranslateText(input);
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

        Assert.IsNotNull(result);
        Assert.AreEqual("¡Hola, mundo!", result.TranslatedText);
    }

    [TestMethod]
    public async Task TranslateFile_NativeStrategy_ShouldReturnTranslatedFile()
    {
        var action = new TranslationActions(InvocationContext, FileManager);
        var input = new Apps.Pangeanic.Models.Requests.Blueprints.TranslateFileRequest
        {
            File = new FileReference
            {
                Name = "sample.txt",
                ContentType = "text/plain"
            },
            SourceLanguage = "en",
            TargetLanguage = "es",
            EngineId = "2154",
            FileTranslationStrategy = "native",
            OutputFileHandling = "original"
        };

        var result = await action.TranslateFile(input);
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.File);
        Assert.AreEqual("sample.txt", result.File.Name);
    }

    [TestMethod]
    public async Task TranslateFile_BlackbirdStrategy_ShouldReturnTranslatedFile()
    {
        var action = new TranslationActions(InvocationContext, FileManager);
        var input = new Apps.Pangeanic.Models.Requests.Blueprints.TranslateFileRequest
        {
            File = new FileReference
            {
                Name = "taus.xliff"
            },
            SourceLanguage = "en",
            TargetLanguage = "es",
            EngineId = "2154",
            FileTranslationStrategy = "blackbird"
        };

        var result = await action.TranslateFile(input);
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.File);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.File.Name));
        Assert.IsTrue(result.File.Name.EndsWith(".xlf", StringComparison.OrdinalIgnoreCase)
            || result.File.Name.EndsWith(".xliff", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void ParseCheckFileResponse_ShouldHandleEmptyArrayResponse()
    {
        var result = ParseCheckFileResponse("[]");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseCheckFileResponse_ShouldHandleRetrieveStylePendingResponse()
    {
        const string response = "{\"data\":null,\"success\":true,\"error\":null,\"status\":\"-20\"}";

        var result = ParseCheckFileResponse(response);

        Assert.IsNotNull(result);
        Assert.AreEqual(-20, result.EffectiveStatus);
        Assert.IsNull(result.EffectiveMessage);
    }

    private static Apps.Pangeanic.Models.Responses.Api.CheckFileResponse? ParseCheckFileResponse(string content)
    {
        var method = typeof(TranslationActions).GetMethod("ParseCheckFileResponse",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "Could not find ParseCheckFileResponse via reflection.");
        return (Apps.Pangeanic.Models.Responses.Api.CheckFileResponse?)method.Invoke(null, [content]);
    }
}
