using System.ComponentModel.DataAnnotations;
using api.posts.Data;
using api.posts.Dtos;
using api.posts.Helpers;
using api.posts.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.posts.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserRepository _repository;
    private readonly JwtService _jwtService;
    private readonly IWebHostEnvironment _env;

    private readonly string[] _imgExt = new[]
    {
        "png",
        "jpg",
        "jpeg",
    };
    private readonly string _path = Path.Combine(new []
    {
        "wwwroot",
        "images",
        "user"
    });

    public AuthController(UserRepository repository, JwtService jwtService, IWebHostEnvironment env)
    {
        _repository = repository;
        _jwtService = jwtService;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(
        [Required][FromForm] string name, 
        [Required][FromForm] string email, 
        [Required][FromForm] string password, 
        IFormFile? image
        )
    {
        var user = new User
        {
            Name = name,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };
        if (image != null)
        {
            var fileExtension = Path.GetExtension(image.FileName);
            var removedExt = fileExtension.Replace(".", "");
            var isContain = _imgExt.Contains(removedExt);
            if (!isContain)
            {
                return BadRequest(new
                {
                    message="format d'image non supporte"
                });
            }

            var uuidPath = name + "_" + Guid.NewGuid() + fileExtension;
            var filePath = Path.Combine(new []
            {
                _env.ContentRootPath,
                _path,
                uuidPath
            });
            await using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            user.ImagePath = Path.Combine(new []
            {
                "images",
                "user",
                uuidPath
            });
        }
        _repository.Create(user);
        return Created("Utilisateur cree avec succes", user);
    }

    [HttpPost("login")]
    public ActionResult Login(LoginDto dto)
    {
        var user = _repository.GetByEmail(dto.Email);

        if (user == null) return BadRequest(new { message = "Invalid credentials" });
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return BadRequest(new { message = "Invalid credentials" });

        var jwt = _jwtService.Generate(user.Id);
        
        // Add to cookies
        Response.Cookies.Append("jwt", jwt,  new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = DateTimeOffset.Now.AddDays(1).TimeOfDay
        });

        return Ok(new
        {
            message="success"
        });
    }

    [HttpGet("user")]
    public ActionResult<User> CurrentUser()
    {
        try
        {
            var jwt = Request.Cookies["jwt"];

            var token = _jwtService.Verify(jwt);
            var userId = int.Parse(token.Issuer);

            var user = _repository.GetById(userId);

            return Ok(user);
        }
        catch (Exception _)
        {
            return Unauthorized();
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");

        return Ok(new
        {
            message = "success"
        });
    }
}