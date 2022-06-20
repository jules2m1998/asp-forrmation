using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.posts.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    [JsonIgnore] public string Password { get; init; } = string.Empty;

    [InverseProperty("User")] public List<Post> Posts { get; set; } = new ();

    [InverseProperty("UserLikes")] public List<Post> PostLikes { get; set; } = new ();


}