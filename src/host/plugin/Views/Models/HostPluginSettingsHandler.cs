using System.Windows;
using Playnite;
using HostPlugin.Models;
using HostPlugin.Common.Extensions;

namespace HostPlugin.Views.Models;

public partial class HostPluginSettingsHandler(Settings settings) : PluginSettingsHandler
{
    public Settings Settings { get => settings; set => settings.Copy(value); }

    private Settings? _copy;

    public override FrameworkElement GetEditView(GetSettingsViewArgs args)
    {
        return new SettingsView { DataContext = this };
    }

    public override async Task BeginEditAsync(BeginEditArgs args)
    {
        _copy = Settings.Clone();
    }

    public override async Task CancelEditAsync(CancelEditArgs args)
    {
        Settings = _copy ?? throw new Exception("BeginEditAsync must be called before CancelEditAsync");
    }

    public override async Task EndEditAsync(EndEditArgs args)
    {
        // Do Nothing
    }
}
