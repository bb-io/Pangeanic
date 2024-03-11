namespace Apps.Pangeanic.Models.Responses;

public class TagsSelectionResponse
{
    public string Type { get; set; }
    
    public Dictionary<string, string> Options { get; set; }
    
    public List<string> Default { get; set; }
    
    public bool Required { get; set; }
}