﻿using System.Net;
using Apps.Pangeanic.Invocables;
using Apps.Pangeanic.Models.Requests;
using Apps.Pangeanic.Webhooks.Models.Responses;
using Apps.Pangeanic.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Webhooks;

[WebhookList]
public class WebhookList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Webhook("On file translation status updated", Description = "Check for updates on file translations. By default, it checks for finished translations. You can specify the status to check for")]
    public Task<WebhookResponse<TranslationsResponse>> OnFileTranslationStatusUpdated(
        WebhookRequest webhookRequest, 
        [WebhookParameter] TranslationStatusUpdatedInput input,
        [WebhookParameter] FileOptionalRequest fileOptionalRequest)
    {
        string translationStatus = input.TranslationStatus ?? "Finished";
        
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
        
        if (translationStatus != data.Data.Message)
        {
            return Task.FromResult(preflightResponse);
        }
        
        if(fileOptionalRequest.FileId != null && fileOptionalRequest.FileId != data.FileId)
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