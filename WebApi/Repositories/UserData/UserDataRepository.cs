using WebApi.Data;
using WebApi.Generics;

namespace WebApi.Repositories.UserData;

public class UserDataRepository : Repository<Data.Entities.UserData, Guid> , IUserDataRepository
{
    public UserDataRepository(ApplicationDbContext context) : base(context) { }
}