using Microsoft.EntityFrameworkCore;
using Server.Models.Database;

namespace Server.Database;

public class OptionsInitializer(WebApplicationBuilder builder)
{
    public void Initialize(DbContextOptionsBuilder options)
    {
        options.UseSqlite(builder.Configuration.GetConnectionString(nameof(DatabaseContext))
                    ?? throw new InvalidOperationException($"Connection string '{nameof(DatabaseContext)}' not found."));
        options.UseAsyncSeeding(SeedAsync);
        options.UseSeeding(Seed);
    }

    private static async Task SeedAsync(
        DbContext context, bool storageOperationPerformed, CancellationToken token)
    {
        await InitializeSetAsync<GameVariantType>(context, InitializeVariants,  token);
        await InitializeSetAsync<Library>        (context, InitializeLibraries, token);
        await InitializeSetAsync<User>            (context, InitializeUsers, token);
        await InitializeSetAsync<CompletionStatus>(context, InitializeCompletionStatus, token);
        var operations = context.Set<Operation>();
        if (operations.Any())
        {
            operations.RemoveRange(operations);
        }
        await context.SaveChangesAsync(token);
    }

    private static void Seed(DbContext context, bool storageOperationPerformed)
    {
        InitializeSet<GameVariantType> (context, InitializeVariants);
        InitializeSet<Library>         (context, InitializeLibraries);
        InitializeSet<User>            (context, InitializeUsers);
        InitializeSet<CompletionStatus>(context, InitializeCompletionStatus);
        var operations = context.Set<Operation>();
        if (operations.Any())
        {
            operations.RemoveRange(operations);
        }
        context.SaveChanges();
    }

    private static async Task InitializeSetAsync<T>(
        DbContext context, Action<DbSet<T>> initAction, CancellationToken token) where T : class
    {
        var set = context.Set<T>();
        if (!await set.AnyAsync(token)) initAction(set);
    }

    private static void InitializeSet<T>( DbContext context, Action<DbSet<T>> initAction) where T : class
    {
        var set = context.Set<T>();
        if (!set.Any()) initAction(set);
    }

    private static void InitializeVariants(DbSet<GameVariantType> gameVariantTypes) => gameVariantTypes.AddRange(
        new() { Name = "Original", GameVariantFlags = GameVariantFlags.Default },
        new() { Name = "Remaster" },
        new() { Name = "Remake" },
        new() { Name = "Port" },
        new() { Name = "Collection", GameVariantFlags = GameVariantFlags.Collection });

    private static void InitializeLibraries(DbSet<Library> libraries) => libraries.Add(new() { Name = "Local" });

    private static void InitializeUsers(DbSet<User> users) => users.Add(new() { UserName = "default"});

    private static void InitializeCompletionStatus(DbSet<CompletionStatus> statuses) => statuses.Add(new() { Name = "Not Started" } );
}
