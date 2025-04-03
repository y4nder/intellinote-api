using WebApi.Generics;

namespace WebApi.Data.Entities;

public class UserData : Entity<Guid>
{
    public String UserId { get; set; }
    public User User { get; set; }
    public List<Folder> Folders { get; set; } = new List<Folder>();
    public List<Note> Notes { get; set; } = new List<Note>();

    public UserData() { }

    private UserData(String userId, User user)
    {
        UserId = userId;
        User = user;
    }

    public static UserData Create(User user)
    {
        return new UserData(user.Id, user);
    }
}