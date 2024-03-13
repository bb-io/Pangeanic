namespace Apps.Pangeanic.Models.Responses;

public class ProcessTextResponse
{
    public List<TranslationPairResponse?> Translations { get; set; } = new();
}