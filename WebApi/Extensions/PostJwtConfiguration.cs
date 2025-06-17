using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace WebApi.Extensions;

public class PostConfigureJwtBearer : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        var existingEvents = options.Events;
        var events = existingEvents ?? new JwtBearerEvents();

        events.OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;

            if (path.StartsWithSegments("/note-hub"))
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                
                if (context.Request.Cookies.TryGetValue("Aufy.AccessToken", out var str))
                    context.Token = str;
                
                // if (context.Request.Cookies.TryGetValue("Aufy.RefreshToken", out var rStr))
                //     context.Token = rStr;
            }
            else
            {
                if (path.StartsWithSegments("/api/auth/signin/refresh"))
                {
                    if (context.Request.Cookies.TryGetValue("Aufy.RefreshToken", out var str))
                        context.Token = str;
                }
                else
                {
                    if (context.Request.Cookies.TryGetValue("Aufy.AccessToken", out var str))
                        context.Token = str;
                }
                
            }
            
            return Task.CompletedTask;
        };

        options.Events = events;
    }
}
