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
    private readonly IUserRepository _repository;
    private readonly JwtService _jwtService;

    public AuthController(IUserRepository repository, JwtService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public ActionResult<User> Register(RegisterDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
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
            HttpOnly = true
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