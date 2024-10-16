﻿using Newtonsoft.Json;

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

    [JsonProperty("runparms")]
    public RunParamsModel? RunParams { get; set; }

    public ProcessTextApiRequest()
    {
        Text = Array.Empty<string>();
    }

    public ProcessTextApiRequest(ProcessTextRequest request, string src, string tgt)
    {
        Src = src;
        Tgt = tgt;
        Engine = request.EngineId;
        GlossaryId = request.GlossaryId != null ? int.Parse(request.GlossaryId) : null;
        Text = request.Text.ToArray();
        RunParams = new RunParamsModel()
        {
            Sensitivity = request.Sensitivity,
            Type = request.Type,
            Tags = request.Tags?.ToList() ?? new List<string>()
        };
    }
    
    public ProcessTextApiRequest(ProcessTextAsStringRequest request, string src, string tgt)
    {
        Src = src;
        Tgt = tgt;
        Engine = request.EngineId;
        GlossaryId = request.GlossaryId != null ? int.Parse(request.GlossaryId) : null;
        Text = [request.Text];
        RunParams = new RunParamsModel()
        {
            Sensitivity = request.Sensitivity?.ToString(),
            Type = request.Type,
            Tags = request.Tags?.ToList()
        };
    }
}