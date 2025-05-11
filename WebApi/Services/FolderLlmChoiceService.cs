using System.Text;
using System.Text.Json;
using OpenAI.Chat;
using WebApi.Data.Entities;
using WebApi.Features.Notes.AutoAssignNote;

namespace WebApi.Services;

public class FolderLlmChoiceService
{
    private readonly ChatClient _chatClient;

    public FolderLlmChoiceService(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<PotentialFolder> GetFolderChoice(List<GetPotentialFolders.FolderScore> folderScores, Note note)
    {
        List<ChatMessage> messages = new()
        {
            new SystemChatMessage("""
                                      You are an intelligent assistant helping organize user notes into folders.
                                      Given a note and a list of candidate folders, your job is to choose the most relevant folder based on the note's content and the folders' summaries.
                                      If no folder is a good fit, reply with "No suitable folder found" and suggest a new folder name or category.
                                  """),

            new UserChatMessage($"""
                                     Below is a note and a list of candidate folders.

                                     Note Title: {note.Title}

                                     Note Summary: {note.Summary}

                                     Note Content:
                                     {note.NormalizedContent}

                                     Candidate Folders:
                                    {string.Join("\n", folderScores.Select(folder => $"""
                                      - Folder Id: {folder.FolderId}
                                      - Folder Title: {folder.FolderName}
                                      - Folder Description: {folder.FolderDescription}
                                      """))}

                                     Please return your answer in the following format:
                                     FolderId: [folder id or null if no folder is a good fit]
                                     BestFolder: [folder title or set as null if no folder is a good fit]
                                     SuggestedFolderName (if needed): [suggested folder name or leave blank]
                                     Reason: [brief explanation, just mention the reason for the choice of folder or leave blank if no folder is a good fit]
                                 """)
        };
        
        var jsonSchema = Encoding.UTF8.GetBytes(@"
                {
                    ""type"": ""object"",
                    ""properties"": {
                        ""folderId"": { ""type"": [""string"", ""null""] },
                        ""folderName"": { ""type"": ""string"" },
                        ""suggestedFolderName"": { ""type"": ""string"" },
                        ""reason"" : { ""type"": ""string"" }
                    },
                    ""required"": [""folderId"", ""folderName"", ""suggestedFolderName"", ""reason""],
                    ""additionalProperties"": false
                }
                ");

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "folder_metadata",
                jsonSchema: BinaryData.FromBytes(jsonSchema),
                jsonSchemaIsStrict: true
            )
        };

        
        
        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
        using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);

        Guid? folderId = null;
        var folderIdElement = structuredJson.RootElement.GetProperty("folderId");
        if (folderIdElement.ValueKind == JsonValueKind.String)
            folderId = Guid.Parse(folderIdElement.GetString()!);

        return new PotentialFolder
        {
            FolderId = folderId,
            FolderName = structuredJson.RootElement.GetProperty("folderName").GetString()!,
            SuggestedFolderName = structuredJson.RootElement.GetProperty("suggestedFolderName").GetString()!,
            Reason = structuredJson.RootElement.GetProperty("reason").GetString()!
        };
    }
}

public class PotentialFolder
{
    public Guid? FolderId { get; set; }
    public string? FolderName { get; set; }
    public string? SuggestedFolderName { get; set; }
    public string Reason { get; set; } = null!;
}