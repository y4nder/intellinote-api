using Aufy.Core.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.Data.Entities;

namespace WebApi.Features.Auth;

public class SignUpExtension : ISignUpEndpointEvents<User, MySignUpRequest>
{
    public Task<ProblemHttpResult?> UserCreatingAsync(MySignUpRequest model, HttpRequest httpRequest, User user)
    {
        // user.AboutMe = model.AboutMe;
        // user.MySiteUrl = model.MySiteUrl;

        return Task.FromResult<ProblemHttpResult?>(null);
    }
}

public class SignUpExternalExtension : ISignUpExternalEndpointEvents<User, MySignUpExternalRequest>
{
    public Task<ProblemHttpResult?> UserCreatingAsync(MySignUpExternalRequest model, HttpRequest httpRequest,
        User user)
    {
        // user.AboutMe = model.AboutMe;
        // user.MySiteUrl = model.MySiteUrl;

        return Task.FromResult<ProblemHttpResult?>(null);
    }
}