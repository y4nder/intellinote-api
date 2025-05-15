using WebApi.Services.Agent.Agents;
using WebApi.Services.Agent.FunctionTools;
using WebApi.Services.Agent.FunctionTools.ToolDefinitions;

namespace WebApi.Services.Agent;

public static class AgentExtensions
{
    private static IServiceCollection AddAgentTools(this IServiceCollection services)
    {
        services.AddScoped<ToolRouter>();
        services.AddScoped<IAgentTool, GetUserNoteTool>();
        services.AddScoped<IAgentTool, GetNoteCitationTool>();
        services.AddScoped<IAgentTool, GetUserFolderTool>();
        services.AddScoped<IAgentTool, GetTopFoldersTool>();
        services.AddScoped<IAgentTool, GetTopNotesTool>();
        return services;
    }

    public static IServiceCollection AddChatAgentsWithTools(this IServiceCollection services)
    {
        services.AddAgentTools();
        services.AddScoped<INoraAgent, Nora>();
        services.AddScoped<ILexAgent, Lex>();
        return services;
    }
}