using RestSharp;

namespace Apps.Pangeanic.Api;

public class PangeanicRequestParameters
{
    public string Url { get; set; }
    
    public Method Method { get; init; }
}