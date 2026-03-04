using Service.Services;
using Piped;
using Service.Common;
using Service.Models;

string path = "";
var attempts = 0;
InitReply reply;
while (true)
{
    reply = await Pipe.SendRequestAsync(RequestType.Init, new InitBody(), InitReply.Parser, CancellationToken.None);
    path = reply.DataPath;
    if (!string.IsNullOrEmpty(path)) break;
    if (attempts++ > 5) throw new ApplicationException("Failed to get Playnite path from host.");

    await Task.Delay(1000);
}

var fullPath = Path.GetFullPath(path);
if (!Path.Exists(fullPath)) throw new ApplicationException($"Playnite path '{fullPath}' does not exist");

var builder = WebApplication.CreateBuilder(args);

builder.Environment.WebRootPath = fullPath;
builder.Environment.ContentRootPath = fullPath;
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(reply.Port - 1);
    o.ListenAnyIP(reply.Port, lo => lo.UseHttps());
});
builder.Services.Configure<MediaPathOptions>(o => o.PlayniteRootPath = builder.Environment.WebRootPath);

var app = builder.Build();
app.MapControllers();
app.UseRouting().UseGrpcWeb();
app.MapGrpcService    <GamesService>().EnableGrpcWeb();
app.MapGrpcService <DatabaseService>().EnableGrpcWeb();
app.MapGrpcService     <TagsService>().EnableGrpcWeb();
app.MapGrpcService<PlatformsService>().EnableGrpcWeb();
app.MapGrpcService   <SeriesService>().EnableGrpcWeb();
app.MapGrpcService <FeaturesService>().EnableGrpcWeb();
app.MapGrpcService<CompaniesService>().EnableGrpcWeb();
app.MapGrpcService<LibrariesService>().EnableGrpcWeb();
app.MapGrpcService      <OpsService>().EnableGrpcWeb();
app.Run();
