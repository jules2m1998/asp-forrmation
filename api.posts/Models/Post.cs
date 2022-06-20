
using System.ComponentModel.DataAnnotations.Schema;

namespace api.posts.Models;

public class Post
{
    public int Id { get; set; }
    public string Msg { get; set; } = string.Empty;
    public string? Image { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public User User { get; set; } = new User();
    public Post? Parent { get; set; }
    public List<Post> Posts { get; set; } = new ();
    public List<User> UserLikes { get; set; } = new();

}