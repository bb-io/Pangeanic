using Apps.Pangeanic.Actions;
using Tests.Pangeanic.Base;

namespace Tests.Pangeanic;

[TestClass]
public class TranslationActionTests : TestBase
{
    [TestMethod]
    public async Task TranslateText_ShouldReturnTranslation()
    {
        // Arrange
        var action = new TranslationActions(InvocationContext);
        var input = new Apps.Pangeanic.Models.Requests.Blueprints.TranslateTextRequest
        {
            Text = "Hello, world!",
            TargetLanguage = "es",
            EngineId= "2154"
        };
        // Act
        var result = await action.TranslateText(input);
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("¡Hola, mundo!", result.TranslatedText);
    }
}

