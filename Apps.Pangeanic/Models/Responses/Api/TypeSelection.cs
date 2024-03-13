namespace Apps.Pangeanic.Models.Responses.Api;

public class TypeSelectionResponse
{
    public string Type { get; set; }
    
    public Dictionary<string, string> Options { get; set; }
    
    public string Default { get; set; }
}