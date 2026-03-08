using Comms.Common.Interface.Models;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Linq;

namespace HostPlugin.Services.RequestHandlers;

public class DatabaseService(IPlayniteAPI playniteApi) : IDatabaseService
{
    public Filter[] GetFilters() 
        => playniteApi.Database.FilterPresets.Select(
            f => new Filter { Id = f.Id, Name = f.Name, Visible = f.ShowInFullscreeQuickSelection }).ToArray();

    public Label[] GetTags()               => CreateLabels(playniteApi.Database.Tags);
    public Label[] GetGenres()             => CreateLabels(playniteApi.Database.Genres);
    public Label[] GetPlatforms()          => CreateLabels(playniteApi.Database.Platforms);
    public Label[] GetCompletionStatuses() => CreateLabels(playniteApi.Database.CompletionStatuses);
    public Label[] GetSources()            => CreateLabels(playniteApi.Database.Sources);
    public Label[] GetFeatures()           => CreateLabels(playniteApi.Database.Features);
    public Label[] GetCompanies()          => CreateLabels(playniteApi.Database.Companies);
    public Label[] GetSeries()             => CreateLabels(playniteApi.Database.Series);

    private static Label[] CreateLabels<T>(IItemCollection<T> objects) where T : DatabaseObject
        => objects.Select(CreateLabel).ToArray();

    private static Label CreateLabel(DatabaseObject obj) => new() { Id = obj.Id, Name = obj.Name };
}