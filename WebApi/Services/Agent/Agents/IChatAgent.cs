#pragma warning disable OPENAI001

using System.ClientModel;
using OpenAI.Assistants;

namespace WebApi.Services.Agent.Agents;

public interface IChatAgent<TPrompt, TResponse>
{
    string AgentName { get; }
    
    Task<TResponse> ProcessPromptAsync(TPrompt prompt);
    
    Task ProcessRequiredActions(AssistantClient assistantClient, ClientResult<ThreadRun> threadRun);
}