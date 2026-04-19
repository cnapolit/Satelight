using Comms.Common.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class DatabaseService(IPlayniteApi playniteApi) : IDatabaseService
{
    public Filter[] GetFilters()
        => [];//playniteApi.MainView.GetCurrentFilters().Select(
           // f => new Filter { Id = f.Id, Name = f.Name, Visible = f.ShowInFullscreeQuickSelection }).ToArray();

    public Label[] GetTags()               => CreateLabels(playniteApi.Library.Tags);
    public Label[] GetGenres()             => CreateLabels(playniteApi.Library.Genres);
    public Label[] GetPlatforms()          => CreateLabels(playniteApi.Library.Platforms);
    public Label[] GetCompletionStatuses() => CreateLabels(playniteApi.Library.CompletionStatuses);
    public Label[] GetSources()            => CreateLabels(playniteApi.Library.Sources);
    public Label[] GetFeatures()           => CreateLabels(playniteApi.Library.Features);
    public Label[] GetCompanies()          => CreateLabels(playniteApi.Library.Companies);
    public Label[] GetSeries()             => CreateLabels(playniteApi.Library.Series);

    private static Label[] CreateLabels<T>(ILibraryCollection<T> objects) where T : LibraryObject
        => objects.Select(CreateLabel).ToArray();

    private static Label CreateLabel(LibraryObject obj) => new() { /*Id = obj.Id,*/ Name = obj.Name };
}