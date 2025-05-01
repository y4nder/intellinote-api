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
                                     Analyze the following combined note titles and summaries, and generate:
                                     1. A short, meaningful title for a folder that would group these notes.
                                     2. A concise description explaining what kind of notes belong in this folder.

                                     Notes content:
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