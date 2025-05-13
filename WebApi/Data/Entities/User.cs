using Aufy.Core;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Data.Entities;

public class User : IdentityUser, IAufyUser
{
    public UserData? Data { get; set; }
    
}

public class AuthorDto
{
    public String Id { get; set; }
    public String UserName { get; set; }
};
