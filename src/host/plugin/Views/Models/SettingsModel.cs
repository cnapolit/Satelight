using System.Collections.Generic;
using HostPlugin.Models;
using Playnite.SDK;
using HostPlugin.Common.Extensions;

namespace HostPlugin.Views.Models;

public class SettingsModel(HostPlugin plugin, Settings? settings) : ObservableObject, ISettings
{
    public Settings? Settings { get => settings; set => settings.Copy(value); }

    private Settings? _copy;
    public void BeginEdit() => _copy = Settings.Clone();
    public void EndEdit() => plugin.SavePluginSettings(Settings);
    public void CancelEdit() => Settings = _copy;

    public bool VerifySettings(out List<string> errors)
    {
        errors = [];
        return true;
    }
}