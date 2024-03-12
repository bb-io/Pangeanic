using Apps.Pangeanic.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Pangeanic.Utils;

public static class CredsHelper
{
    public static string GetUrl(this AuthenticationCredentialsProvider[] creds)
    {
        var url = creds.First(x => x.KeyName == CredsNames.Url).Value;
        if (url.EndsWith($"/"))
        {
            url = url.Remove(url.Length - 1, 1);
        }
        
        return url;
    }
    
    public static string GetToken(this AuthenticationCredentialsProvider[] creds)
    {
        return creds.First(x => x.KeyName == CredsNames.ApiKey).Value;
    }
}