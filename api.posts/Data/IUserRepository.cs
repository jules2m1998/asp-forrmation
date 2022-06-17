using api.posts.Models;

namespace api.posts.Data;

public interface IUserRepository
{
    User Create(User user);
    User? GetByEmail(string email);
    User? GetById(int id);
}