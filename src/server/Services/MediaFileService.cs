
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Common.Utility.Extensions;
using Microsoft.Extensions.Options;
using Server.Models;
using Satelight.Protos.Host;
using Server.Services.Host;

namespace Server.Services;

public class MediaFileService(
    ILogger<MediaFileService> logger,
    IOptions<Settings> options,
    IHttpClientFactory httpClientFactory,
    ChannelManager channelManager)
{
    private const string CoversDir = "Covers";
    private const string ScreenshotsDir = "Screenshots";
    private const string VideosDir = "Videos";
    private const string IconsDir = "Icons";
    private const string BackgroundsDir = "Backgrounds";
    private const string LogosDir = "Logos";
    private const string NoLogosDir = "No-Logos";
    private const string MusicDir = "Music";
    private static readonly string LogosCoversDir = Path.Combine(CoversDir, LogosDir);
    private static readonly string NoLogosCoversDir = Path.Combine(CoversDir, NoLogosDir);

    private static string GetHostUrl(Models.Database.Host host) => $"https://{host.Ip}:{host.Port}";
    private static string GetHostGameUrl(Models.Database.Host host, Guid hostGameId) => $"{GetHostUrl(host)}/games/{hostGameId}";
    private static string GetHostCoverUrl(Models.Database.Host host, Guid hostGameId) => $"{GetHostGameUrl(host, hostGameId)}/covers";
    private static string GetHostBackgroundUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/backgrounds";
    private static string GetHostScreenshotUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/screenshots";
    private static string GetHostTrailerUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/trailer";
    private static string GetHostMicroTrailerUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/microTrailer";
    private static string GetHostMusicUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/music";
    private static string GetHostLogoUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/logo";
    private static string GetHostIconUrl(Models.Database.Host host, Guid hostGameId)
        => $"{GetHostGameUrl(host, hostGameId)}/icon";

    public Task DownloadCoverAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => DownloadImagesAsync(host, hostGameId, gameId, GetHostCoverUrl(host, hostGameId), GetCoverPath(gameId), LogosCoversDir, token);
    public Task DownloadBackgroundAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => DownloadImagesAsync(host, hostGameId, gameId, GetHostBackgroundUrl(host, hostGameId), GetBackgroundPath(gameId), BackgroundsDir, token);
    public ValueTask DownloadTrailerAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => DownloadTrailerAsync(gameId, GetHostTrailerUrl(host, hostGameId), "Trailer.mp4", token);
    public ValueTask DownloadMicroTrailerAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => DownloadTrailerAsync(gameId, GetHostMicroTrailerUrl(host, hostGameId), "MicroTrailer.mp4", token);
    public ValueTask DownloadLogoAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => DownloadFileAsync(GetHostLogoUrl(host, hostGameId), GetLogoPath(gameId), "Logo.png", token);
    public ValueTask DownloadIconAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
        => DownloadFileAsync(GetHostIconUrl(host, hostGameId), GetIconPath(gameId), "Icon.ico", token);

    public async Task DownloadMusicAsync(Models.Database.Host host, Guid hostGameId, Guid gameId, CancellationToken token)
    {
        var url = GetHostMusicUrl(host, hostGameId);
        var httpClient = httpClientFactory.CreateClient("DefaultClient");
        var response = await httpClient.GetAsync(new Uri(url), token);
        if (!response.IsSuccessStatusCode) return;
        
        var musicDir = GetMusicPath(gameId);
        var json = await response.Content.ReadAsStreamAsync(token);
        var files = await JsonSerializer.DeserializeAsync<List<string>>(json, cancellationToken: token) ?? [];
        await files.Select(f => DownloadMusicAsync(httpClient, url, musicDir, f, token)).WhenAllAsync();
    }

    public IEnumerable<string> GetCovers(Guid gameId) => GetFiles(gameId, CoversDir);
    public IEnumerable<string> GetCovers(Guid gameId, int width, int height)
        => GetFiles(gameId, Path.Combine(LogosCoversDir, GetAspectRatioDir(width, height)));
    public IEnumerable<string> GetLogolessCovers(Guid gameId) => GetFiles(gameId, NoLogosCoversDir);
    public IEnumerable<string> GetBackgrounds(Guid gameId) => GetFiles(gameId, BackgroundsDir);
    public IEnumerable<string> GetScreenshots(Guid gameId) => GetFiles(gameId, ScreenshotsDir);
    public IEnumerable<string> GetVideos(Guid gameId) => GetFiles(gameId, VideosDir);
    public IEnumerable<string> GetIcons(Guid gameId) => GetFiles(gameId, IconsDir);
    public IEnumerable<string> GetLogos(Guid gameId) => GetFiles(gameId, LogosDir);
    public IEnumerable<string> GetMusic(Guid gameId) => GetFiles(gameId, MusicDir);

    private IEnumerable<string> GetFiles(Guid gameId, string subPath)
    {
        var path = Path.Combine(options.Value.ContentPath, gameId.ToString(), subPath);
        // TODO: Use configuration for URL
        var urlPath = "http://192.168.1.153:5224/media";
        return Directory.Exists(path) 
             ? Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                        .Select(f => f.Replace(options.Value.ContentPath, urlPath).Replace('\\', '/'))
             : [];
    }
    private string GetFile(Guid gameId, string subPath)
    {
        var path = Path.Combine(options.Value.ContentPath, gameId.ToString(), subPath);
        // TODO: Use configuration for URL
        var urlPath = "http://192.168.1.153:5224/media";
        return path.Replace(options.Value.ContentPath, urlPath).Replace('\\', '/');
    }

    private static async ValueTask DownloadMusicAsync(HttpClient httpCient, string url, string dirPath, string file, CancellationToken token)
    {
        var filePath = Path.Combine(dirPath, file);
        if (File.Exists(filePath)) return;

        var imageResponse = await httpCient.GetAsync(new Uri(url + $"/{file}"), token);
        if (!imageResponse.IsSuccessStatusCode) return;
        
        var response = await httpCient.GetAsync(new Uri(url), token);
        if (!response.IsSuccessStatusCode) return;

        var video = await response.Content.ReadAsStreamAsync(token);
        Directory.CreateDirectory(dirPath);
        await using var fileStream = File.Create(filePath);
        await video.CopyToAsync(fileStream, token);
    }

    private async ValueTask DownloadTrailerAsync(Guid gameId, string url, string fileName, CancellationToken token)
    {
        var dirPath = Path.Combine(GetVideoPath(gameId), VideosDir);
        await DownloadFileAsync(url, dirPath, fileName, token);
    }

    private async ValueTask DownloadFileAsync(string url, string dir, string fileName, CancellationToken token)
    {
        var filePath = Path.Combine(dir, fileName);
        if (File.Exists(filePath)) return;

        var httpCient = httpClientFactory.CreateClient("DefaultClient");
        var response = await httpCient.GetAsync(new Uri(url), token);
        if (!response.IsSuccessStatusCode) return;

        var video = await response.Content.ReadAsStreamAsync(token);
        Directory.CreateDirectory(dir);
        await using var fileStream = File.Create(filePath);
        await video.CopyToAsync(fileStream, token);
    }

    private async Task DownloadImagesAsync(
        Models.Database.Host host,
        Guid hostGameId,
        Guid gameId,
        string url,
        string imageDir,
        string serverSubPath,
        CancellationToken token)
    {
        var httpCient = httpClientFactory.CreateClient("DefaultClient");
        var response = await httpCient.GetAsync(new Uri(url), token);
        if (!response.IsSuccessStatusCode) return;

        var json = await response.Content.ReadAsStreamAsync(token);
        var files = await JsonSerializer.DeserializeAsync<List<string>>(json, cancellationToken: token) ?? [];
        Directory.CreateDirectory(imageDir);
        var existingFiles = Directory.GetFiles(imageDir);
        await Task.WhenAll(files.Select(f => DownloadImageAsync(httpCient, url, imageDir, f, token)));
        return;

        Media.MediaClient client = new(channelManager.GetChannel(host));
        var updateHostFilesTasks =
        from   file in Directory.GetFiles(imageDir)
        where  !existingFiles.Contains(file)
        select client.UpdateCoverAsync(new()
        {
            UpdateFileInfo = new()
            {
                Game = hostGameId.ToByteString(),
                FileName = Path.GetFileName(file),
                NewPath = GetFile(gameId, serverSubPath)
            }
        }, cancellationToken: token);
           
        foreach (var task in updateHostFilesTasks.ToArray()) await task;
    }

    private static async Task DownloadImageAsync(
        HttpClient httpCient, string url, string imageDir, string file, CancellationToken token)
    {
        var imageResponse = await httpCient.GetAsync(new Uri(url + $"/{file}"), token);
        if (!imageResponse.IsSuccessStatusCode) return;

        using var image = await GetImageAsync(imageResponse, token);
        var (aspectWidth, aspectHeight) = GetAspectRatio(image);
        var dirPath = Path.Combine(imageDir, GetAspectRatioDir(aspectWidth, aspectHeight));
        var filePath = Path.Combine(dirPath, file);
        if (File.Exists(filePath)) return;

        Directory.CreateDirectory(dirPath);
        await using var fileStream = File.Create(filePath);
        await image.SaveAsPngAsync(fileStream, token);
    }

    private string GetMusicPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), MusicDir);
    private string GetCoverPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), CoversDir);
    private string GetBackgroundPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), BackgroundsDir);
    private string GetGamePath(Guid gameId)
        => Path.Combine(options.Value.ContentPath, gameId.ToString());
    private string GetLogoPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), LogosDir);
    private string GetIconPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), IconsDir);
    private string GetScreenshotPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), ScreenshotsDir);
    private string GetVideoPath(Guid gameId)
        => Path.Combine(GetGamePath(gameId), VideosDir);

    private static async Task<Image> GetImageAsync(HttpResponseMessage imageResponse, CancellationToken token)
    {
        await using var imageStream = await imageResponse.Content.ReadAsStreamAsync(token);
        return await Image.LoadAsync<Rgba32>(imageStream, token);
    }

    private static string GetAspectRatioDir(int width, int height)
        => $"{width}x{height}";

    private static (int, int) GetAspectRatio(Image image)
    {
        var (greatestDenominator, newDenominator)
            =  image.Width < image.Height 
            ? (image.Width,  image.Height)
            : (image.Height, image.Width);
        while (newDenominator != 0)
        {
            var previousDenominator = greatestDenominator;
            greatestDenominator = newDenominator;
            newDenominator = previousDenominator % newDenominator;
        }
        return (image.Width / greatestDenominator, image.Height / greatestDenominator);
    }
}