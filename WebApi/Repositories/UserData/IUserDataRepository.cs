using WebApi.Generics;

namespace WebApi.Repositories.UserData;

public interface IUserDataRepository : IRepository<Data.Entities.UserData, Guid>;