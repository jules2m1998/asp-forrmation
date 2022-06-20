namespace api.posts.Dtos;

public class AddPostDto
{
    public string Msg { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int UserId { get; set; }
}