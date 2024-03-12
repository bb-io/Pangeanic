using System.Net;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Webhooks.Models.Responses;
using Apps.Pangeanic.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Webhooks;

[WebhookList]
public class WebhookList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Webhook("On file translation status updated", Description = "Check for updates on file translations")]
    public Task<WebhookResponse<TranslationsResponse>> OnFileTranslationStatusUpdated(
        WebhookRequest webhookRequest, [WebhookParameter]TranslationStatusUpdatedInput input)
    {
        var data = JsonConvert.DeserializeObject<TranslationStatusUpdatedPayload>(webhookRequest.Body.ToString());
        if (data is null)
        {
            throw new InvalidCastException(nameof(webhookRequest.Body));
        }
        
        var preflightResponse = new WebhookResponse<TranslationsResponse>
        {
            HttpResponseMessage = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
        
        if (!string.IsNullOrEmpty(input.TranslationStatus) && input.TranslationStatus != data.Data.Message)
        {
            return Task.FromResult(preflightResponse);
        }

        return Task.FromResult(new WebhookResponse<TranslationsResponse>()
        {
            HttpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK },
            ReceivedWebhookRequestType = WebhookRequestType.Default,
            Result = new TranslationsResponse
            {
                FileId = data.FileId,
                Status = data.Data.Message
            }
        });
    }
}