using Aufy.Core;
using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;

namespace WebApi.Features.Auth;

public class LogOutUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout-user", async (HttpContext context) =>
        {
            // Sign out cookie-based auth schemes
            await context.SignOutAsync("Aufy.BearerSignInCookieScheme");
            context.Response.Cookies.Delete("Aufy.AccessToken");
            context.Response.Cookies.Delete("Aufy.RefreshToken");
            return Results.NoContent();
        }).RequireAuthorization();

        app.MapGet("/debug/auth-schemes", async ([FromServices] IAuthenticationSchemeProvider schemes) =>
        {
            var allSchemes = await schemes.GetAllSchemesAsync();
            return new
            {
                schemes = allSchemes.Select(s => s.Name)
            };
        });
    }
}