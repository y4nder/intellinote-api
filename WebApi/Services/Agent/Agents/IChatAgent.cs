#pragma warning disable OPENAI001

namespace WebApi.Services.Agent.Agents;

public interface IChatAgent<TPrompt, TResponse>
{
    string AgentName { get; }
    
    Task<TResponse> ProcessPromptAsync(TPrompt prompt);
}