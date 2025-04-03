using System.Security.Claims;
using Aufy.Core;
using Microsoft.AspNetCore.Identity;
using WebApi.Errors.ErrorDefinitions;
using WebApi.Extensions;

namespace WebApi.Services;

public class UserContext<TEntity, TKey> where TEntity : IdentityUser, IAufyUser, new() where TKey : IEquatable<TKey>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<TEntity> _userManager;

    public UserContext(IHttpContextAccessor contextAccessor, UserManager<TEntity> userManager)
    {
        _httpContextAccessor = contextAccessor;
        _userManager = userManager;
    }

    public TKey Id()
    {
        var result = IdExtractor();
        return (TKey)Convert.ChangeType(result, typeof(TKey));
    }

    public async Task<TEntity> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
        return user!;
    }
    
    private string IdExtractor()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if ((user is null) || (!user.Identity?.IsAuthenticated ?? true))
        {
            throw UserErrors.UserNotAuthenticated.AsThrowableException();
        }

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw UserErrors.UserNotAuthenticated.AsThrowableException();
        }
        
        return userId;
    }
}