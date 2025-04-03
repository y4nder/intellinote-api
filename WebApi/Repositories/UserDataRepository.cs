using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories;

public class UserDataRepository : Repository<UserData, Guid>
{
    public UserDataRepository(ApplicationDbContext context) : base(context) { }
}