using api.posts.Models;
using Microsoft.EntityFrameworkCore;

namespace api.posts.Data;

public class UserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public User Create(User user)
    {
        _context.Users.Add(user);
        user.Id = _context.SaveChanges();

        return user;
    }

    public User? GetByEmail(string email)
    {
        return _context.Users
            .AsNoTracking()
            .Include(u => u.Posts)
            .FirstOrDefault(u => u.Email == email);
    }

    public User? GetById(int id)
    {
        return _context.Users
            .AsNoTracking()
            .Include(u => u.Posts)
            .FirstOrDefault(u => u.Id == id);
    }
}