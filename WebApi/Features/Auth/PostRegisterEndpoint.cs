using Aufy.Core;
using Aufy.Core.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.Data.Entities;
using WebApi.Repositories;
using WebApi.Repositories.UserData;
using WebApi.Services;

namespace WebApi.Features.Auth;

public static class PostRegisterEndpoint
{
    public static void UseCreateUserDataAction(this RouteHandlerBuilder routeHandlerBuilder)
    {
        routeHandlerBuilder.AddEndpointFilter(async (context, next) =>
        {
            var result = await next(context);
            
            if (result is Results<Ok<SignUpResponse>, BadRequest, ProblemHttpResult>)
            {
                var request = context.GetArgument<SignUpRequest>(0);
                var userService = context.HttpContext.RequestServices.GetRequiredService<AufyUserManager<User>>();
                var user = await userService.FindByEmailAsync(request.Email!)??
                           throw new KeyNotFoundException("User not found.");
                        
                var newUserData = UserData.Create(user);
                        
                var userDataRepository = context.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();
                var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<UnitOfWork>();
            
                userDataRepository.Add(newUserData);
                await unitOfWork.Commit();
            }
                    
            return result;
        });
    }
}