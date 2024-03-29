using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace Apps.Pangeanic.Api;

public class PangeanicClient : RestClient
{
    public async Task<T> ExecuteRequestAsync<T>(string endpoint, Method method, BaseJsonRequest? bodyObj,
        AuthenticationCredentialsProvider[] creds)
    {
        bodyObj!.ApiKey = creds.GetToken();
        var response = await ExecuteWithJson(endpoint, method, bodyObj, creds);
        
        return JsonConvert.DeserializeObject<T>(response.Content);
    }

    private async Task<RestResponse> ExecuteWithJson(string endpoint, Method method, object? bodyObj,
        AuthenticationCredentialsProvider[] creds)
    {
        var url = creds.GetUrl();
        
        var request = new RestRequest(url + endpoint, method);
        if (bodyObj is not null)
        {
            request.WithJsonBody(bodyObj, new()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        return await ExecuteRequest(request);
    }
    
    public async Task<RestResponse> ExecuteRequest(RestRequest request)
    {
        var response = await ExecuteAsync(request);

        if (!response.IsSuccessStatusCode)
            throw GetError(response);

        return response;
    }
    
    private Exception GetError(RestResponse response)
    {
        return new Exception($"Status code: {response.StatusCode}, Content: {response.Content}");
    }
}