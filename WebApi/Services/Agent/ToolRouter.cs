using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using OpenAI.Assistants;
using WebApi.Services.Agent.FunctionTools;

namespace WebApi.Services.Agent;

public class ToolRouter
{
    private readonly IEnumerable<IAgentTool> _agentTools;

    public ToolRouter(IEnumerable<IAgentTool> agentTools)
    {
        _agentTools = agentTools;
    }

    [Experimental("OPENAI001")]
    public async Task<ToolOutput> RouteToolAsync(RequiredAction action)
    {
        var tool = _agentTools.FirstOrDefault(x => x.FunctionName == action.FunctionName);
        if (tool == null)
            throw new ArgumentNullException(nameof(tool), "Tool not found.");
        
        return await tool.ProcessAsync(action);
    }
}