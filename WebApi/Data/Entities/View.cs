using Newtonsoft.Json;
using WebApi.Features.Views;
using WebApi.Generics;

namespace WebApi.Data.Entities;

public class View : Entity<Guid>
{
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FilterCondition { get; set; } = null!;
    
    public View() { }

    public static View Create(User user, string name, string filterCondition)
    {
        return new View
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = name,
            User = user,
            FilterCondition = filterCondition
        };
    }

    public static View CreateManually(User user, string name, List<string> topics)
    {
        var conditions = topics.Select(topic => new
        {
            id = DateTime.UtcNow.Ticks.ToString(), // more unique, timestamp-based
            property = "topics",
            @operator = "contains",
            value = new[] { topic }
        });
        
        string jsonCondition = JsonConvert.SerializeObject(conditions);

        return new View
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = name,
            User = user,
            FilterCondition = jsonCondition
        };
    }

    public void Update(string? name, string? filterCondition)
    {
        if (name is null && filterCondition is null) return;
        
        if (!string.IsNullOrEmpty(name))
        {
            if(!name.Equals(Name)) Name = name;
        }

        if (!string.IsNullOrEmpty(filterCondition))
        {
            if(!filterCondition.Equals(FilterCondition)) FilterCondition = filterCondition;
        }
        
        SetUpdated();
    }
}

public class ViewResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FilterCondition { get; set; } = null!;    
}