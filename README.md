# Blackbird.io Pangeanic

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Pangeanic produces custom AI models so you can create your own Generative AI (GenAI) with no data transfer to third parties.

## Before setting up

Before you can connect you need to make sure that:

- You must have API key for Pangeanic services

If you don't have an API key, you can contact [Pangeanic](https://pangeanic.com/translation-technology/translate-easy).

## Connecting 

1. Navigate to Apps, and identify the **Pangeanic** app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference, e.g., 'My client'.
4. In the _API Key_ field, input your API Key.
5. In the _URL_ field, input URL of your api. For example: http://prod.pangeamt.com:8080/
5. Click _Connect_.
6. Confirm that the connection has appeared and the status is _Connected_.

![PangeanicConnection](image/README/PangeanicConnection.png)

## Actions

### Text

- **Process text**: Sends text to be processed and translated as a single string

- **Process text as array**: Sends text to be processed and translated as an array of strings

In the process text actions, you have the option to specify anonymization settings, including sensitivity, type, and tags.

### File

- **Process file**: Sends file to the API for processing and translation, and then retrieves the response containing the File ID

- **Download file**: Download translated file based on file ID

## Events

### Translations

- **On file translation status updated**: Check for updates on file translations. By default, it checks for finished translations. You can specify the status to check for

## Examples

With this app you can build the following birds:

- First bird: 

![PangeanicExample1](image/README/PangeanicExample1.png)

- Second bird:

![PangeanicExample2](image/README/PangeanicExample2.png)

In short, these birds allow you to upload files to Google Drive, translate them using Pangeanic, and once the file is translated, upload it to wherever you want (in this case, Dropbox).

Also, note that to use the Pangeanic 'On file translation status updated' event, you should publish it and then copy the 'Webhook URL':

![Pangeanic-callback-url](image/README/Pangeanic-callback-url.png)

Paste this URL into the 'Callback URL' optional input of the 'Process file' action:

![Pangeanic-callback-url2](image/README/Pangeanic-callback-url2.png)

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
