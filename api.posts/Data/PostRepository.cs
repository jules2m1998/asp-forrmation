using api.posts.Dtos;
using api.posts.Helpers;
using api.posts.Models;
using Microsoft.EntityFrameworkCore;

namespace api.posts.Data;

public class PostRepository
{
    private readonly DataContext _context;

    public PostRepository(DataContext context)
    {
        _context = context;
    }

    public Post Create(string msg, int userId, int? postId, string? imagePath = null)
    {
        var u = _context.Users.Find(userId);
        
        if (u == null) throw new NotFoundException("utilisateur inexistant");
        var p = new Post
        {
            Msg = msg,
            CreatedAt = DateTimeOffset.Now,
            Image = imagePath,
            User = u
        };

        if (postId != null)
        {
            var post = _context.Posts.Find(postId);

            if (post == null) throw new NotFoundException("post inexistant");

            p.Parent = post;
        }

        _context.Posts.Add(p);
        _context.SaveChanges();
        
        return p;
    }

    public Post Update(Post post)
    {
        _context.Posts.Update(post);

        return post;
    }

    public Post Delete(Post post)
    {
        _context.Posts.Remove(post);
        _context.SaveChanges();

        return post;
    }

    public Post? GetById(int id)
    {
        return _context.Posts.Include(p => p.User).FirstOrDefault(p => p.Id == id);
    }

    public List<Post> GetByUserId(int userId)
    {
        return _context.Posts
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Posts)
            .Include(s => s.UserLikes)
            .Where(p => p.User.Id == userId && p.Parent == null)
            .OrderByDescending(p => p.Id)
            .ToList();
    }

    public List<Post> GetAll()
    {
        return _context.Posts
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Posts)
            .Include(s => s.UserLikes)
            .OrderByDescending(p => p.Id)
            .ToList();
    }

    public List<Post> GetChildren(int id)
    {
        return _context.Posts
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Posts)
            .Include(s => s.UserLikes)
            .Where(p => p.Parent != null && p.Parent.Id == id)
            .OrderByDescending(p => p.Id)
            .ToList();
    }
}