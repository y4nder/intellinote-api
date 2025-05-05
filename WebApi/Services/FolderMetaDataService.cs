using System.Text.Json;
using OpenAI.Chat;

namespace WebApi.Services;

public class FolderMetaDataService
{
    private readonly ChatClient _client;

    public FolderMetaDataService(ChatClient client)
    {
        _client = client;
    }

    public async Task<FolderMetadataDto> GenerateFolderMetadata(string text)
    {
        List<ChatMessage> messages =
        [
            new UserChatMessage($"""
                                     Please analyze the following combined note titles and summaries and generate:
                                     1. A short, meaningful title for a folder that effectively groups these notes, ensuring that it is relevant to the content and provides a clear understanding of the theme.
                                     2. A concise yet informative description of the folder, starting with a brief introduction to the folder's theme. Then, explain the types of notes that belong in this folder and why they are grouped together. Be sure to highlight key topics or subjects covered by the notes.
                                     
                                     The notes content is as follows:
                                     {text}
                                 """)
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "folder_metadata",
                jsonSchema: BinaryData.FromBytes("""
                                                 {
                                                     "type": "object",
                                                     "properties": {
                                                         "title": { "type": "string" },
                                                         "description": { "type": "string" }
                                                     },
                                                     "required": ["title", "description"],
                                                     "additionalProperties": false
                                                 }
                                                 """u8.ToArray()),
                jsonSchemaIsStrict: true
            )
        };

        ChatCompletion completion = await _client.CompleteChatAsync(messages, options);

        using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);

        string title = structuredJson.RootElement.GetProperty("title").GetString()!;
        string description = structuredJson.RootElement.GetProperty("description").GetString()!;

        return new FolderMetadataDto { Title = title, Description = description };
    }

}

public class FolderMetadataDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}