using Apps.Pangeanic.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Pangeanic.Invocables;

public class AppInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected PangeanicClient Client { get; }
    public AppInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }
}