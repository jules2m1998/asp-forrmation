namespace api.posts.Helpers;

public static class FileHelper
{
    private static readonly string MyPath = Path.Combine(new []
    {
        "wwwroot",
        "images"
    });
    
    private static readonly string[] _imgExt = new[]
    {
        "png",
        "jpg",
        "jpeg",
    };

    public static async Task<string?> CreateFile(IFormFile image, string name, IWebHostEnvironment env, string outFile)
    {
        var fileExtension = Path.GetExtension(image.FileName);
        var removedExt = fileExtension.Replace(".", "");
        var isContain = _imgExt.Contains(removedExt);
        
        if (!isContain)
        {
            return null;
        }
        
        var uuidPath = name + "_" + Guid.NewGuid() + fileExtension;
        var filePath = Path.Combine(new []
        {
            env.ContentRootPath,
            MyPath,
            outFile,
            uuidPath
        });
        await using (Stream fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return Path.Combine(new []
        {
            "images",
            outFile,
            uuidPath
        });
    }
}