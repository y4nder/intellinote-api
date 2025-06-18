using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Repositories.View;

public interface IViewRepository : IRepository<Data.Entities.View, Guid>
{
    Task<List<ViewResponseDto>> GetViewsByUserId(string userId);
    Task<ViewResponseDto?> GetViewDtoByIdAsync(Guid viewId, string userId);
    Task<Data.Entities.View?> GetViewByIdAsync(Guid viewId, string userId);
}