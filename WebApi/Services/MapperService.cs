using AutoMapper;
using WebApi.Data.Entities;

namespace WebApi.Services;

public class MapperService : Profile
{
    public MapperService()
    {
        CreateMap<Note, NoteDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => src.Folder))
            .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Keywords))
            .ReverseMap();
        CreateMap<Note, NoteDtoMinimal>()
            .ForMember(d => d.Keywords, opt => opt.MapFrom(src => src.Keywords))
            .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => src.Folder));

        CreateMap<Note, NoteDtoVeryMinimal>();
        
        CreateMap<NoteDto, NoteDtoMinimal>()
            .ForMember(d => d.Keywords, opt => opt.MapFrom(src => src.Keywords))
            .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => src.Folder));
        
        CreateMap<Folder, FolderDto>().ReverseMap();
        CreateMap<Folder, FolderWithDetailsDto>()
            .ForMember(d => d.Author, opt => opt.MapFrom(src => src.User))
            .ForMember(d => d.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Keywords));

        CreateMap<Folder, FolderWithDetailsDtoMinimal>();
        CreateMap<Folder, FolderWithoutDetailsDto>();
        
        CreateMap<User, AuthorDto>().ReverseMap();
        
        CreateMap<Keyword, KeywordDto>().ReverseMap();
    }
}

public static class MapperServiceExtensions2
{
    public static void AddMapperService(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MapperService));
    }
}