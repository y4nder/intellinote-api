using Carter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;
using WebApi.Extensions;
using WebApi.Services;

namespace WebApi.Features.Auth;

public class GetCurrentUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/me", async ([FromServices] UserContext<User, string> userContext) => {
            var user = await userContext.GetCurrentUser();
            return new CurrentUserResponse {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = []
            };
        })
        .AddProducedTypes<CurrentUserResponse>()
        .RequireAuthorization();
    }

    internal class CurrentUserResponse {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
