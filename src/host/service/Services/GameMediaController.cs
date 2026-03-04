using Common.Utility.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Piped;
using Service.Common;
using Service.Common.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Google.Protobuf;
using Service.Models;
using Google.Protobuf.Collections;

namespace Service.Services;

[ApiController]
[Route("Games/{gameId:guid}")]
public class GameMediaController(ILogger<GameMediaController> logger, IOptions<MediaPathOptions> options) : ControllerBase
{
    private const           string MusicFolder = "Music Files";
    private static readonly string BackgroundChangerJsonPath;
    private static readonly string BackgroundChangerImagePath;
    private static readonly string ExtraMetadataGamePath = Path.Combine("Extrametadata", "Games");
    private static readonly string LibraryFilesPath      = Path.Combine("Library",       "Files");

    static GameMediaController()
    {
        var backgroundChangerSubPath = Path.Combine("ExtensionsData", "3afdd02b-db6c-4b60-8faa-2971d6dfad2a");
        BackgroundChangerJsonPath  = Path.Combine(backgroundChangerSubPath, "BackgroundChanger");
        BackgroundChangerImagePath = Path.Combine(backgroundChangerSubPath, "Images");
    }


    [HttpGet("Covers")]
    public async Task<IActionResult> GetCoversAsync(Guid gameId, CancellationToken token)
    {
        var gameCover = await GetGameCoverAsync(gameId, token);
        if (gameCover is null) return NotFound($"Game files for Id '{gameId}' do not exist");

        List<string> files = [];
        switch (gameCover)
        {
            case null: return NotFound($"Game '{gameId}' does not exist");
            //case "":   break;
            default:   files.AddRange(gameCover.Select(Path.GetFileName)!); break;
        }


        await foreach (var filePath in GetBackgroundChangerImagesAsync(gameId, true, token))
        {
            var fileName = Path.GetFileName(filePath);
            if (!files.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                files.Add(fileName);
            }
        }

        return Ok(files);
    }

    [HttpGet("Covers/{fileName:file}")]
    public async Task<IActionResult> GetCoverAsync(Guid gameId, string fileName, CancellationToken token)
    {
        var gameCovers = await GetGameCoverAsync(gameId, token);
        switch (gameCovers)
        {
            case null: return NotFound($"Game '{gameId}' does not exist");
            default:
                var gameCover = gameCovers.FirstOrDefault(c => Path.GetFileName(c).Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (gameCover != null)
                    return PhysicalFile(gameCover, ImageToMimeType(fileName));
                break;
        }

        await foreach (var imagePath in GetBackgroundChangerImagesAsync(gameId, true, token))
        if            (Path.GetFileName(imagePath).Equals(fileName, StringComparison.OrdinalIgnoreCase))
        return        PhysicalFile(imagePath, ImageToMimeType(fileName));

        return NotFound();
    }

    private Task<RepeatedField<string>?> GetGameCoverAsync(Guid gameId, CancellationToken token)
        => GetPlayniteImagesAsync(gameId, GetPlayniteCoverAsync, token);
    private async Task<RepeatedField<string>> GetPlayniteCoverAsync(ByteString gameId, CancellationToken token)
    {
        GetGameCoversBody getGameCoverRequest = new() { Id = gameId };
        var reply = await Pipe.SendRequestAsync(
            logger, RequestType.GetGameCover, getGameCoverRequest, GetGameCoversReply.Parser, token);
        return reply.Covers;
    }

    private Task<RepeatedField<string>?> GetGameBackgroundAsync(Guid gameId, CancellationToken token)
        => GetPlayniteImagesAsync(gameId, GetPlayniteBackgroundAsync, token);

    private async Task<RepeatedField<string>> GetPlayniteBackgroundAsync(ByteString gameId, CancellationToken token)
    {
        GetGameBackgroundsBody getGameBackgroundRequest = new() { Id = gameId };
        var reply = await Pipe.SendRequestAsync(
            logger, RequestType.GetGameBackground, getGameBackgroundRequest, GetGameBackgroundsReply.Parser, token);
        return reply.Backgrounds;
    }

    private async Task<RepeatedField<string>?> GetPlayniteImagesAsync(
        Guid gameId, Func<ByteString, CancellationToken, Task<RepeatedField<string>>> getImagesFunc, CancellationToken token)
    {
        var gamePath = GetGameLibraryPath(gameId);
        if (!Directory.Exists(gamePath)) return null;

        return await getImagesFunc(gameId.ToByteString(), token);
        //if (id.IsEmpty) return string.Empty;

        //var backgroundId = id.ToGuid().ToString();
        //return Directory.GetFiles(gamePath).First(f => Path.GetFileName(f).StartsWith(backgroundId));
    }

    private string GetGameLibraryPath(Guid gameId)
        => Path.Combine(options.Value.PlayniteRootPath, LibraryFilesPath, gameId.ToString());

    private async IAsyncEnumerable<string> GetBackgroundChangerImagesAsync(
        Guid gameId, bool expectsCover, [EnumeratorCancellation] CancellationToken token)
    {
        var gameIdStr = gameId.ToString();
        if      (TryGetFile(out var jsonPath, options.Value.PlayniteRootPath, BackgroundChangerJsonPath, $"{gameIdStr}.json")
              && TryGetDir (out var imagesPath, options.Value.PlayniteRootPath, BackgroundChangerImagePath, gameIdStr))
        using   (var json = await GetAsync(jsonPath, token))
        if      (json.RootElement.ValueKind is JsonValueKind.Object)
        foreach (var item in json.RootElement.AsArr("Items"))
        if      (item.ValueKind is JsonValueKind.Object
              && item.IsFalse  ("IsDefault")
              && item.IsTrue   ("IsCover") == expectsCover
              && item.TryGetStr("Name", out var name)
              && TryGetFile(out var filePath, imagesPath, name))
        yield return filePath;
        
    }

    [HttpGet("Backgrounds")]
    public async Task<IActionResult> GetBackgroundsAsync(Guid gameId, CancellationToken token)
    {
        List<string> files = [];
        var background = await GetGameBackgroundAsync(gameId, token);
        switch (background)
        {
            case null: return NotFound($"Game '{gameId}' does not exist");
            default:
                files.AddRange(background.Select(Path.GetFileName)!);
                break;
            //case "":   break;
            //default:   files.Add(Path.GetFileName(background)); break;
        }
        //await foreach (var filePath in GetBackgroundChangerImagesAsync(gameId, false, token))
        //{
        //    var fileName = Path.GetFileName(filePath);
        //    if (!files.Contains(fileName, StringComparer.OrdinalIgnoreCase))
        //    {
        //        files.Add(fileName);
        //    }
        //}

        return Ok(files);
    }

    [HttpGet("Backgrounds/{fileName:file}")]
    public async Task<IActionResult> GetBackgroundAsync(Guid gameId, string fileName, CancellationToken token)
    {
        var background = await GetGameBackgroundAsync(gameId, token);
        switch (background)
        {
            case null: return NotFound($"Game '{gameId}' does not exist");
            //case "":   break;
            default:
                var gameCover = background.FirstOrDefault(c => Path.GetFileName(c).Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (gameCover != null)
                    return PhysicalFile(gameCover, ImageToMimeType(fileName));
                break;
        }

        //await foreach (var p in GetBackgroundChangerImagesAsync(gameId, false, token))
        //if            (Path.GetFileName(p).Equals(fileName, StringComparison.OrdinalIgnoreCase))
        //return        PhysicalFile(p, ImageToMimeType(fileName));

        return NotFound();
    }

    [HttpGet("Icon")]
    public IActionResult GetIcon(Guid gameId)
    {
        var libraryPath = GetGameLibraryPath(gameId);
        if (Directory.Exists(libraryPath))
        {
            var file = Directory.GetFiles(libraryPath).FirstOrDefault(f => f.EndsWith(".ico"));
            if (file != null)
            {
                return PhysicalFile(file, "image/vnd.microsoft.icon");
            }
        }
        return NotFound();
    }

    [HttpGet("Trailer")]
    public IActionResult GetTrailer(Guid gameId) => GetExtraMetadataFile(gameId, "Trailer.mp4", "video/mp4");

    [HttpGet("MicroTrailer")]
    public IActionResult GetMicroTrailer(Guid gameId) => GetExtraMetadataFile(gameId, "MicroTrailer.mp4", "video/mp4");

    [HttpGet("Logo")]
    public IActionResult GetLogo(Guid gameId) => GetExtraMetadataFile(gameId, "Logo.png", "image/png");

    private IActionResult GetExtraMetadataFile(Guid gameId, string fileName, string mimeType)
        => TryGetFile(out var filePath, GetExtraMetadataGamePath(gameId), fileName)
         ? PhysicalFile(filePath, mimeType)
         : NotFound();

    [HttpGet("Music")]
    public IActionResult GetMusic(Guid gameId)
    {
        var filePath = GetGameMusicPath(gameId);
        return Directory.Exists(filePath) ? Ok(Directory.GetFiles(filePath)) : NotFound();
    }

    private static string GetGameMusicPath(Guid gameId)
        => Path.Combine(GetExtraMetadataGamePath(gameId), MusicFolder);

    private static string GetExtraMetadataGamePath(Guid gameId)
        => Path.Combine(ExtraMetadataGamePath, gameId.ToString());

    [HttpGet("Music/{fileName:file}")]
    public IActionResult GetMusic(Guid gameId, string fileName)
        => GetAbsFile(GetGameMusicPath(gameId), fileName);

    [HttpGet("Audio")]
    public IActionResult GetAudio(Guid gameId)
    {
        throw new NotImplementedException();
    }

    private IActionResult GetAbsFile(params string[] segments)
    {
        var dir = Path.Combine(segments[..^1]);
        return !TryGetFile(out var filePath, dir, segments[^1]) ? NotFound()
             : IsNotRelativePath(filePath, dir)                 ? Forbid() 
                                                                : PhysicalFile(filePath, AudioToMimeType(filePath));
    }

    private static string ImageToMimeType(string filePath) => Path.GetExtension(filePath).ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png"            => "image/png",
        ".webp"           => "image/webp",
        _                  => "application/octet-stream"
    };

    private static string AudioToMimeType(string filePath) => Path.GetExtension(filePath).ToLower() switch
    {
        ".wav" => "audio/wav",
        ".mp3" => "audio/mpeg"
    };

    private static bool TryGetFile(out string filePath, params string[] segments)
    {
        filePath = Path.Combine(segments);
        return System.IO.File.Exists(filePath);
    }

    private static bool TryGetDir(out string dirPath, params string[] segments)
    {
        dirPath = Path.Combine(segments);
        return Directory.Exists(dirPath);
    }

    private static bool IsNotRelativePath(string path, string parentDir)
        => !Path.GetFullPath(path).StartsWith(parentDir, StringComparison.OrdinalIgnoreCase);

    private static async Task<JsonDocument> GetAsync(string path, CancellationToken token)
    {
        FileStreamOptions fileOptions = new()
        {
            Share   = FileShare.ReadWrite,
            Access  = FileAccess.Read,
            Mode    = FileMode.Open,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan
        };
        await using var fileStream = System.IO.File.Open(path, fileOptions);

        JsonDocumentOptions jsonOptions = new()
        {
            AllowTrailingCommas = true,
            CommentHandling     = JsonCommentHandling.Skip,
            MaxDepth            = 4
        };
        return await JsonDocument.ParseAsync(fileStream, jsonOptions, token);
    }
}