using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApi.Features.Auth;

public class LogOutUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout-user", async (HttpContext context) =>
        {
            // Mirror your cookie settings exactly (Secure, SameSite=None)
            var cookieOpts = new CookieOptions 
            {
                Path     = "/", 
                Domain   = context.Request.Host.Host,
                HttpOnly = true,
                Secure   = true,              // Required for SameSite=None
                SameSite = SameSiteMode.None, // Explicitly allow cross-site
                Expires  = DateTimeOffset.UtcNow.AddDays(-1) // Expire immediately
            };

            // Overwrite cookies to delete them
            context.Response.Cookies.Append("Aufy.RefreshToken", "", cookieOpts);
            context.Response.Cookies.Append("Aufy.AccessToken", "", cookieOpts);

            // Perform sign-out for any active schemes
            await context.SignOutAsync("Aufy.BearerSignInCookieScheme");
            await context.SignOutAsync("Aufy.BearerSignInTokenScheme");

            return Results.NoContent();
        }).RequireAuthorization();
        
    }
}