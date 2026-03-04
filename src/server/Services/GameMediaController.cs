using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Server.Models;

namespace Server.Services;

[ApiController]
[Route("media")]
public class GameMediaController(ILogger<GameMediaController> logger, IOptions<Settings> options) : ControllerBase
{
    [HttpGet("List/{*subPath}")]
    public IActionResult List(string subPath)
    {
        var basePath = Path.GetFullPath(options.Value.ContentPath);
        var path = Path.Combine(basePath, subPath);
        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath) || !fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        return Ok(Directory.GetFiles(fullPath).Select(Path.GetFileName));
    }

    [HttpGet("{id:guid}/{mediaType}/{subPath?}")]
    public IActionResult Get(Guid id, string mediaType, string? subPath)
    {
        var basePath = Path.GetFullPath(options.Value.ContentPath);
        var path = Path.Combine(basePath, id.ToString(), mediaType, subPath ?? string.Empty);

        var fileExists = System.IO.File.Exists(path);
        var dirExists = Directory.Exists(path);
        if ((!fileExists && !dirExists) || !Path.GetFullPath(path).StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        if (dirExists)
        {
            path = Directory.GetFiles(path, "*", SearchOption.AllDirectories).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(path))
            {
                return NotFound();
            }
        }

        var mimeType = FileToMimeType(path);
        return PhysicalFile(path, mimeType);
    }

    private static string FileToMimeType(string filePath) => Path.GetExtension(filePath).TrimStart('.') switch
    {
        "jpg" or "jpeg" => "image/jpeg",
        "png"           => "image/png",
        "gif"           => "image/gif",
        "mp4"           => "video/mp4",
        "webm"          => "video/webm",
        _               => "application/octet-stream"
    };
}