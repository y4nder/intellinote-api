using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Features.Folders;

public class FolderContracts
{
    public record GetFoldersRequest() : IMappable<GetFolders.Query>;
    
    public record GetFoldersResponse(List<FolderWithDetailsDto> Folders);
    
    public record GetFolderResponse(FolderWithDetailsDto Folder);

    public record CreateFolderRequest(
        String Name,
        String Description
    ) : IMappable<CreateFolder.Command>;
    
    public record CreateFolderResponse(FolderWithDetailsDto Folder);

    public record UpdateFolderRequest(
        String? Name,
        String? Description
    ): IMappable<UpdateFolder.Command>;
    
    public record UpdateFolderResponse(FolderWithDetailsDto Folder);
    
    public record DeleteFolderResponse(Guid FolderId);    
}