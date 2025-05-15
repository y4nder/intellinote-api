using Newtonsoft.Json;
using OpenAI.Chat;
using WebApi.Data.Entities;

namespace WebApi.Services;

public class TopicExtractorService
{
    private readonly ChatClient _client;

    public TopicExtractorService(ChatClient client)
    {
        _client = client;
    }

    public async Task<ExtractedTopicsDto> ExtractTopics(string query, List<NoteDtoWithTopics> notes)
    {
        var notesSummary = string.Join("\n", notes.Select(n =>
            $"- Title: {n.Title}\n  Topics: {string.Join(", ", n.Topics)}"));

        List<ChatMessage> messages = new()
        {
            new SystemChatMessage("You are a helpful assistant that analyzes note topics."),
            new UserChatMessage($"""
                You are given a user query and a list of notes. Each note has a title and a list of topics.

                Your job is to:
                - Analyze the query and compare it to the list of notes.
                - Disregard any notes that are not relevant to the query.
                - From the relevant notes, extract and return the most relevant topics that match the query.
                - Only use the topics that are explicitly listed in the notes.
                - Avoid duplicates and limit the list to highly relevant topics only (maximum of 30).
                - Additionally, generate a short and descriptive title (3-6 words) that represents the view based on the selected topics.

                Query:
                "{query}"

                Notes:
                {notesSummary}

                Return your answer in JSON format:
            """)
        };

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "relevant_topics",
                jsonSchema: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "name": { "type": "string" },
                        "topics": {
                            "type": "array",
                            "items": { "type": "string" }
                        }
                    },
                    "required": ["name", "topics"],
                    "additionalProperties": false
                }
                """u8.ToArray()),
                jsonSchemaIsStrict: true
            )
        };

        ChatCompletion completion = await _client.CompleteChatAsync(messages, options);
        var responseMessage = completion.Content.First().Text;
        var result = JsonConvert.DeserializeObject<ExtractedTopicsDto>(responseMessage);
        return result!;
    }


}

public class ExtractedTopicsDto
{
    public string Name { get; set; } = null!;
    public List<string> Topics { get; set; } = null!;
}