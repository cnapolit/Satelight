using CommunityToolkit.Mvvm.ComponentModel;

namespace HostPlugin.Models;

public class Settings : ObservableObject
{
    public int Port { get; set; } = 5156;
}