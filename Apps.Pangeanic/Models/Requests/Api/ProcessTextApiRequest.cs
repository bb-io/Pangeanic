using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Requests.Api;

public class ProcessTextApiRequest : BaseJsonRequest
{
    [JsonProperty("src")]
    public string Src { get; set; }
    
    [JsonProperty("tgt")]
    public string Tgt { get; set; }

    [JsonProperty("engine")]
    public string Engine { get; set; }

    [JsonProperty("glossary_id")]
    public int? GlossaryId { get; set; }

    [JsonProperty("text")]
    public string[] Text { get; set; }

    public ProcessTextApiRequest()
    {
        Text = Array.Empty<string>();
    }

    public ProcessTextApiRequest(ProcessTextRequest request)
    {
        Src = request.Source;
        Tgt = request.Target;
        Engine = request.EngineId;
        GlossaryId = request.GlossaryId != null ? int.Parse(request.GlossaryId) : null;
        Text = request.Text.ToArray();
    }
}