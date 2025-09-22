using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{**imagePath}")] // <-- підтримка довільного шляху з підкаталогами
    public IActionResult GetImage(string imagePath)
    {
        // Розкодуємо пробіли та інші символи
        var decodedPath = Uri.UnescapeDataString(imagePath);

        // Повний шлях до файлу (заміни на свою базову директорію)
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MUSICAPI", decodedPath);

        Console.Write(fullPath);

        if (!System.IO.File.Exists(fullPath))
            return NotFound("Файл не знайдено.");

        var contentType = GetContentType(fullPath);
        var fileBytes = System.IO.File.ReadAllBytes(fullPath);
        return File(fileBytes, contentType);
    }

    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}
