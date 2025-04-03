using WebApi.Features.SampleFeature.Events;
using WebApi.Generics;

namespace WebApi.Data.Entities;

public class SampleEntity : Entity<Guid>
{
    public SampleEntity() {}

    private SampleEntity(Guid id, string name, string uniqueName, string email)
    {
        Id = id;
        Name = name;
        UniqueName = uniqueName;
        Email = email;
    }
    
    public string Name { get; set; } = null!;    
    public string UniqueName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public static SampleEntity Create(string name, string uniqueName, string email)
    {
        if (name.Length == 0) throw new ArgumentException("Name cannot be empty", nameof(name));
        SampleEntity s = new(Guid.NewGuid(), name, uniqueName, email);

        var domainEvent = new SampleCreatedEvent
        {
            Email = s.Email
        };
        
        s.AddDomainEvent(domainEvent);

        return s;
    }

    public void Update(SampleEntity sample)
    {
        if (this.Name != sample.Name)
        {
            this.Name = sample.Name;
        };

        if (this.UniqueName != sample.UniqueName)
        {
            this.UniqueName = sample.UniqueName;
        }
        
        SetUpdated();
    } 
}