using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Threading;
using System.Windows.Controls;
using Comms.Common.Interface.Models;
using Comms.Host.Factory;
using HostPlugin.Common;
using HostPlugin.Models;
using HostPlugin.Services;
using HostPlugin.Services.RequestHandlers;
using HostPlugin.Views;
using HostPlugin.Views.Models;

namespace HostPlugin;

public class HostPlugin : GenericPlugin
{
    public override Guid Id => Constants.PluginId;

    private readonly Settings                _settings;
    private readonly Server                  _server;
    private readonly ActionTracker           _actionTracker;
    private readonly CancellationTokenSource _tokenSource = new();

    public HostPlugin(IPlayniteAPI playniteApi) : base(playniteApi)
    {
        Properties     = new() { HasSettings = true };
        _settings      = LoadPluginSettings<Settings>() ?? new();
        _actionTracker = new();
        RequestHandlerFactory requestHandlerFactory = new(playniteApi, _actionTracker, new(playniteApi), new(playniteApi));
        _server = new(HostFactory.CreateListener(), requestHandlerFactory);
    }

    public override void Dispose()
    {
        _tokenSource.Cancel();
        _server.Dispose();
    }

    public override void OnApplicationStarted(OnApplicationStartedEventArgs args) 
        => _server.Run(_tokenSource.Token);

    public override void OnApplicationStopped(OnApplicationStoppedEventArgs args) 
        => _tokenSource.Cancel();

    public override ISettings GetSettings(bool firstRunSettings) => new SettingsModel(this, _settings);
    public override UserControl GetSettingsView(bool firstRunView) => new SettingsView();

    public override void OnGameInstalled(OnGameInstalledEventArgs args)
        => _actionTracker.UpdateOp(args.Game.Id, RequestType.InstallGame, OpState.Finished);

    public override void OnGameStarted(OnGameStartedEventArgs args)
        => _actionTracker.UpdateOp(args.Game.Id, RequestType.StartGame, OpState.Finished);

    public override void OnGameStarting(OnGameStartingEventArgs args)
        => _actionTracker.UpdateOp(args.Game.Id, RequestType.StartGame, OpState.Running);

    public override void OnGameStartupCancelled(OnGameStartupCancelledEventArgs args)
        => _actionTracker.UpdateOp(args.Game.Id, RequestType.StartGame, OpState.Failed);

    public override void OnGameStopped(OnGameStoppedEventArgs args)
        => _actionTracker.UpdateOp(args.Game.Id, RequestType.StopGame, OpState.Finished);

    public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        => _actionTracker.UpdateOp(args.Game.Id, RequestType.UninstallGame, OpState.Finished);

}