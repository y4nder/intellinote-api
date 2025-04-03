
using WebApi.Generics;

namespace WebApi.Features.SampleFeature;

public class UpdateSampleContract : IMappable<UpdateSample.Command>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;    
    public string UniqueName { get; set; } = null!;
}