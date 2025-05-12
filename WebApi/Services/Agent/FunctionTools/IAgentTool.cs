#pragma warning disable OPENAI001
using OpenAI.Assistants;

namespace WebApi.Services.Agent.FunctionTools;

public interface IAgentTool
{
    string FunctionName { get; }
    Task<ToolOutput> ProcessAsync(RequiredAction action);
}