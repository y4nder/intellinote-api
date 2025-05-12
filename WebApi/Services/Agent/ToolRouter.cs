#pragma warning disable OPENAI001
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
    
    public async Task<ToolOutput> RouteToolAsync(RequiredAction action)
    {
        var tool = _agentTools.FirstOrDefault(x => x.FunctionName == action.FunctionName);
        if (tool == null)
            throw new ArgumentNullException(nameof(tool), "Tool not found.");
        
        return await tool.ProcessAsync(action);
    }
}