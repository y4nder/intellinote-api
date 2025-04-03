using WebApi.Features.SampleFeature;
using WebApi.Generics;

namespace WebApi.Contracts;

public class CreateSampleRequest : IMappable<CreateSample.Command>
{
    public string Name { get; set; } = null!;
    public string UniqueName { get; set; } = null!;

    public string Email { get; set; } = null!;
}