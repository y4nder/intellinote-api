#pragma warning disable OPENAI001
using System.ClientModel;
using OpenAI.Assistants;

namespace WebApi.Services.Agent.FunctionTools;

public interface IAgentTool
{
    string FunctionName { get; }
    Task<ToolOutput> ProcessAsync(RequiredAction action);
}