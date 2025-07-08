using Apps.Pangeanic.Constants;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Models.Requests.Api;
using Apps.Pangeanic.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System.Text;
using System.Text.RegularExpressions;

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

        var content = response.RawBytes != null
              ? Encoding.UTF8.GetString(response.RawBytes)
              : string.Empty;

        if (response.ContentType?.Contains("html") == true
        || content.TrimStart().StartsWith("<"))
        {
            var htmlErrorMessage = ExtractHtmlErrorMessage(content);
            throw new PluginApplicationException(
                $"Expected JSON but received HTML response. {htmlErrorMessage}");
        }

        if (!response.IsSuccessStatusCode)
            throw GetError(response);

        return response;
    }

    private string ExtractHtmlErrorMessage(string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent))
            return "Empty HTML response received.";

        try
        {
            var titleMatch = Regex.Match(htmlContent, @"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : null;

            var h1Match = Regex.Match(htmlContent, @"<h1[^>]*>(.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var h1 = h1Match.Success ? StripHtmlTags(h1Match.Groups[1].Value).Trim() : null;

            var errorPatterns = new[]
            {
                @"<[^>]*class[^>]*error[^>]*>(.*?)</[^>]*>",
                @"<[^>]*class[^>]*message[^>]*>(.*?)</[^>]*>",
                @"<p[^>]*>(.*?)</p>",
                @"<div[^>]*>(.*?)</div>"
            };

            string errorMessage = null;
            foreach (var pattern in errorPatterns)
            {
                var match = Regex.Match(htmlContent, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
                {
                    errorMessage = StripHtmlTags(match.Groups[1].Value).Trim();
                    if (errorMessage.Length > 10)
                        break;
                }
            }

            var messageParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(title) && !title.Contains("DOCTYPE") && !title.Contains("html"))
                messageParts.Add($"Page title: {title}");

            if (!string.IsNullOrWhiteSpace(h1) && h1 != title)
                messageParts.Add($"Error: {h1}");

            if (!string.IsNullOrWhiteSpace(errorMessage) && errorMessage != title && errorMessage != h1)
                messageParts.Add($"Details: {errorMessage}");

            if (messageParts.Any())
                return string.Join(" | ", messageParts);

            var cleanContent = StripHtmlTags(htmlContent).Trim();
            if (cleanContent.Length > 200)
                cleanContent = cleanContent.Substring(0, 200) + "...";

            return string.IsNullOrWhiteSpace(cleanContent)
                ? "Received HTML response without readable content."
                : $"HTML content: {cleanContent}";
        }
        catch (Exception ex)
        {
            return $"Could not parse HTML response. Error: {ex.Message}";
        }
    }

    private string StripHtmlTags(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var withoutTags = Regex.Replace(html, @"<[^>]*>", " ");

        withoutTags = System.Net.WebUtility.HtmlDecode(withoutTags);
        withoutTags = Regex.Replace(withoutTags, @"\s+", " ");

        return withoutTags.Trim();
    }

    private Exception GetError(RestResponse response)
    {
        return new PluginApplicationException($"Status code: {response.StatusCode}, Content: {response.Content}");
    }
}