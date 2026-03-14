using Microsoft.Extensions.FileProviders;
using Server.Database;
using Server.Models;
using Server.Services;
using Server.Services.Images.WebProcessors;
using Server.Services.Protos;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Server.Blazor;
using Server.Services.Host;
using Server.Models.UserInterface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

OptionsInitializer optionsInitializer = new(builder);
builder.Services.AddDbContextFactory<DatabaseContext>(optionsInitializer.Initialize);

// Add services to the container.
builder.Services
       .AddRazorComponents()
       .AddInteractiveServerComponents();
builder.Services.AddHttpClient("DefaultClient").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // TODO: fix
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});
builder.Services
       .AddPlayniteWebClient()
       .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:3000/api"));
builder.Services.AddGrpc();
builder.Services.AddImageSharp().AddProcessor<BlurProcessor>().AddProcessor<SaturationProcessor>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebAppCors", policy =>
    {
        policy
           .AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});
builder.Services
       .AddSingleton<IDatabaseClient, SatelightDatabaseClient>()
       .AddSingleton<HostOperationPollingService>()
       .AddSingleton<ChannelManager>()
       .AddSingleton<MediaFileService>()
       .AddSingleton<HostClient>()
       .AddHostedService(sp => sp.GetRequiredService<HostOperationPollingService>())
       .AddScoped<BrowseState>()
       .AddBlazorise( o =>
       {
           o.Immediate = true;
       })
       .AddBootstrap5Providers()
       .AddFontAwesomeIcons()
       .Configure<Settings>(o => o.ContentPath = Path.Combine("wwwroot", "media"));
var app = builder.Build();

await using (var dbContext = await app.Services.GetRequiredService<IDbContextFactory<DatabaseContext>>().CreateDbContextAsync())
{
    dbContext.Operations.RemoveRange(dbContext.Operations);
    await dbContext.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection()
   .UseCors("WebAppCors")
   .UseRouting()
   .UseAntiforgery()
   .UseGrpcWeb()
   .UseImageSharp()
   .UseStaticFiles()
   .UseStaticFiles(new StaticFileOptions
   {
       FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "media"))
   });
app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapGrpcService    <GamesService>().EnableGrpcWeb();
app.MapGrpcService <DatabaseService>().EnableGrpcWeb();
app.MapGrpcService<PlatformsService>().EnableGrpcWeb();
app.MapGrpcService   <GenresService>().EnableGrpcWeb();
app.MapGrpcService     <TagsService>().EnableGrpcWeb();
app.MapGrpcService      <OpsService>().EnableGrpcWeb();
app.MapGrpcService  <FiltersService>().EnableGrpcWeb();
await app.RunAsync();
