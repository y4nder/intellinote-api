using WebApi.Data.Entities;
using WebApi.Generics;

namespace WebApi.Features.Folders;

public class FolderContracts
{
    public record GetFoldersRequest() : IMappable<GetFolders.Query>;
    
    public record GetFoldersResponse(
        List<FolderWithDetailsDto> Folders,
        int TotalCount
    );
    
    public record GetFolderResponse(FolderWithDetailsDto Folder);

    public record CreateFolderRequest(
        String Name,
        String Description,
        List<Guid> NoteIds,
        bool Auto
    ) : IMappable<CreateFolder.Command>;
    
    public record CreateFolderResponse(FolderWithDetailsDto Folder, bool IsGenerating);

    public record UpdateFolderRequest(
        String? Name,
        String? Description
    ): IMappable<UpdateFolder.Command>;
    
    public record UpdateFolderResponse(FolderWithDetailsDto Folder);
    
    public record DeleteFolderResponse(Guid FolderId);
    
    public static string[] AllowedActionTypes = ["add", "delete"];

    public record UpdateFolderNotesRequest(
        List<Guid> NoteIds);
}