using System.Runtime.Serialization;

namespace api.posts.Helpers;

public class NotFoundException : Exception
{

    public NotFoundException(string? message) : base(message)
    {
    }
}