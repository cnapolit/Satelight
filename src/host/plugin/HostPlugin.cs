using Autofac;
using HostPlugin.Models;
using HostPlugin.Services;
using HostPlugin.Views.Models;
using Playnite;
using Comms.Host.Interface;
using Comms.Host.Factory;
using Comms.Common.Interface.Models;
using Game = Playnite.Game;

namespace HostPlugin;

public sealed class HostPlugin : Playnite.Plugin
{

    private Settings _settings = new();
    private IServer? _server;
    private IActionTracker? _actionTracker;
    private IContainer? _container;
    private readonly CancellationTokenSource _tokenSource = new();

    // Same ID as the one specified in "extension.toml" manifest.
    // You will need to reference this when comparing some objects and requests
    // that have specific plugin owner. To check if it's relevant to you.
    public const string Id = "cnapolit.Satelight";


    // Any initialization related to the SDK and Playnite functionality should be handled here.
    public override async Task InitializeAsync(InitializeArgs args)
    {
        Loc.Api = args.Api;

        var builder = new ContainerBuilder();
        // External instances / factories that can't be assembly-scanned
        builder.RegisterInstance(args.Api).As<IPlayniteApi>();
        builder.RegisterInstance(args.Api.Library).As<ILibraryApi>();
        builder.Register(_ => HostFactory.CreateListener()).As<IHostListener>().SingleInstance();

        // Auto-register all service types by their implemented interfaces
        builder.RegisterAssemblyTypes(typeof(HostPlugin).Assembly)
               .AsImplementedInterfaces()
               .SingleInstance()
               .PropertiesAutowired();

        _container = builder.Build();
        _actionTracker = _container.Resolve<IActionTracker>();
        _server = _container.Resolve<IServer>();
    }

    // Following are descriptions of specific method features implementations.

    // You should remove any methods you are actually not implementing.

    public override async ValueTask DisposeAsync()
    {
        _tokenSource.Cancel();
        var disposeTask = _container?.DisposeAsync() ?? ValueTask.CompletedTask;
        await disposeTask;
        _tokenSource.Dispose();
    }

    public override async Task<CollectDiagnosticDataArgsAsyncResult?> CollectDiagnosticDataArgsAsync(CollectDiagnosticDataArgs args)
    {
        // Implement this method if you want to gather custom data when user generates diagnostics data for your plugin.
        // This can be run manually by user from addons view or on crash dialog that detected your plugin to be the source of the crash.
        // If the method is missing, Playnite collects extension log.
        return await base.CollectDiagnosticDataArgsAsync(args);
    }

    // Implement this if you want to provide settings view functionality for your plugin that will be shown on addons views.
    public override async Task<PluginSettingsHandler?> GetSettingsHandlerAsync(GetSettingsHandlerArgs args)
    {
        // Check PluginSettingsHandler implementation for more details.
        return new HostPluginSettingsHandler(_settings);
    }

    // Same method are available for other built-in collection, like for example OnGenreCollectionChange.
    public override async Task OnGameCollectionChange(DataCollectionChangeArgs<Game> args)
    {
        // This is called when data in the collection are changed.
        // args.AddedItems, args.RemovedItems, args.UpdatedItems
    }

    public override async Task OnApplicationStartupAsync(OnApplicationStartupArgs args)
    {
        await Task.CompletedTask;
        _server?.Run(_tokenSource.Token);
    }

    public override async Task OnApplicationShutdownAsync(OnApplicationShutdownArgs args)
    {
        await Task.CompletedTask;
        _tokenSource.Cancel();
    }

    public override async Task OnGameInstalledAsync(OnGameInstalledEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.Game.Id, RequestType.InstallGame, OpState.Finished);
    }

    public override async Task OnGameInstallationCancelledAsync(OnGameInstallationCancelledEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.Game.Id, RequestType.InstallGame, OpState.Failed);
    }

    public override async Task OnGameStartedAsync(OnGameStartedEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.StartingArgs.Game.Id, RequestType.StartGame, OpState.Finished);
    }

    public override async Task OnGameStartingAsync(OnGameStartingEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.Game.Id, RequestType.StartGame, OpState.Running);
    }

    public override async Task OnGameStartupCancelledAsync(OnGameStartupCancelledEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.SessionArgs.Game.Id, RequestType.StartGame, OpState.Failed);
    }

    public override async Task OnGameStoppedAsync(OnGameStoppedEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.StartingArgs.Game.Id, RequestType.StopGame, OpState.Finished);
    }

    public override async Task OnGameUninstalledAsync(OnGameUninstalledEventArgs args)
    {
        await Task.CompletedTask;
        _actionTracker?.UpdateOp(args.Game.Id, RequestType.UninstallGame, OpState.Finished);
    }

    public override Task OnGameUninstallationCancelledAsync(OnGameUninstallationCancelledEventArgs args)
    {
        return base.OnGameUninstallationCancelledAsync(args);
    }

}