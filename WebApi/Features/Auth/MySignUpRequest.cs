using Aufy.Core.Endpoints;

namespace WebApi.Features.Auth;

public interface IMySignUpRequest
{
}

public class MySignUpRequest : SignUpRequest, IMySignUpRequest
{
}

public class MySignUpExternalRequest : SignUpExternalRequest, IMySignUpRequest
{
}