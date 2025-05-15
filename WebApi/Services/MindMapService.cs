using OpenAI.Chat;

namespace WebApi.Services;

public class MindMapService
{
    private readonly ChatClient _chatClient;

    public MindMapService(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GenerateMindMap(string text)
    {
        List<ChatMessage> messages = new()
        {
            new SystemChatMessage("""
                                      You are a helpful assistant that transforms normalized notes into markdown formatted for mind maps.
                                      Use a structure compatible with tools like Markmap:
                                      - Begin with a single H1 heading (#) that captures the overall topic.
                                      - Use nested bullet points (-) to organize ideas hierarchically.
                                      - Avoid numbered lists, tables, or advanced markdown features.
                                      - Do not include any commentary or explanations—just the raw markdown.
                                  """),
            new UserChatMessage($"""
                                     Convert the following normalized note into a markdown mind map:

                                     {text}
                                 """)
        };
        
        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);
        var responseMessage = completion.Content.First().Text;
        return responseMessage;
    }
}