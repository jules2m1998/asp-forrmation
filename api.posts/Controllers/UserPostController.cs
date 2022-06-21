using System.ComponentModel.DataAnnotations;
using api.posts.Data;
using api.posts.Helpers;
using api.posts.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.posts.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserPostController: ControllerBase
{
    private readonly PostRepository _repository;
    private readonly UserRepository _userRepository;
    private readonly IWebHostEnvironment _env;
    private readonly JwtService _jwtService;

    public UserPostController(UserRepository userRepository, IWebHostEnvironment env, PostRepository repository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _env = env;
        _repository = repository;
        _jwtService = jwtService;
    }

    [HttpPost("")]
    public async Task<ActionResult<Post>> Create(
        [Required][FromForm] string msg,
        [Required][FromForm] int userId,
        [FromForm] int? postId,
        IFormFile? image
    )
    {
        Post? post;

        try
        {
            if (image == null)
            {
                post = _repository.Create(msg, userId, postId);
            }
            else
            {
                var path = await FileHelper.CreateFile(image, userId.ToString(), _env, "post");
                if (path == null) return BadRequest("Format de fichier non pris en charge");

                post = _repository.Create(msg, userId, postId, path);
            }
        }
        catch (NotFoundException e)
        {
            return BadRequest(new
            {
                message=e.Message
            });
        }

        return Created("post enregistrer", post);
    }

    [HttpGet("")]
    public ActionResult<List<Post>?> GetAll(int userId)
    {
        return Ok(_repository.GetByUserId(userId));
    }

    [HttpDelete("")]
    public ActionResult Delete(int id)
    {
        try
        {
            var jwt = Request.Cookies["jwt"];
            var token = _jwtService.Verify(jwt);
            var userId = int.Parse(token.Issuer);
            var post = _repository.GetById(id);

            if (post?.User.Id != userId) return Unauthorized();

            _repository.Delete(post);
            return NoContent();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return Unauthorized();
        }
    }
}