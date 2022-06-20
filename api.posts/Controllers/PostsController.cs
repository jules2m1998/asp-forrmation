using api.posts.Data;
using api.posts.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.posts.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController: ControllerBase
{
    private readonly PostRepository _repository;
    private readonly UserRepository _userRepository;
    private readonly IWebHostEnvironment _env;

    public PostsController(PostRepository repository, UserRepository userRepository, IWebHostEnvironment env)
    {
        _repository = repository;
        _userRepository = userRepository;
        _env = env;
    }

    [HttpGet("all")]
    public ActionResult<List<Post>> GetAllPosts()
    {
        return Ok(_repository.GetAll());
    }
}