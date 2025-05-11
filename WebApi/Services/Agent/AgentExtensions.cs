using WebApi.Services.Agent.Agents;
using WebApi.Services.Agent.Dtos;
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
        return services;
    }

    public static IServiceCollection AddChatAgentsWithTools(this IServiceCollection services)
    {
        services.AddAgentTools();
        services.AddScoped<IChatAgent<PromptContracts.PromptRequestDto, PromptContracts.PromptResponseDto>, Nora>();
        return services;
    }
}