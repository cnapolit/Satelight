using Microsoft.EntityFrameworkCore;
using Server.Models.Database;
using Host = Server.Models.Database.Host;

namespace Server.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Game>             Games                { get; set; }
    public DbSet<GameVariant>      GamesVariants        { get; set; }
    public DbSet<GameVariantType>  GameVariantTypes     { get; set; }
    public DbSet<LibraryGame>      LibraryGames         { get; set; }
    public DbSet<Host>             Hosts                { get; set; }
    public DbSet<HostGame>         HostGames            { get; set; }
    public DbSet<User>             Users                { get; set; }
    public DbSet<UserGameInfo>     UserGameInfo         { get; set; }
    public DbSet<UserGameSession>  UserGameSessions     { get; set; }
    public DbSet<Account>          Accounts             { get; set; }
    public DbSet<Library>          Libraries            { get; set; }
    public DbSet<LibraryAlias>     LibraryAliases       { get; set; }
    public DbSet<GameOwnership>    GamesOwnedByAccounts { get; set; }
    public DbSet<Ownership>        Ownerships           { get; set; }
    public DbSet<CompletionStatus> CompletionStatuses   { get; set; }
    public DbSet<Company>          Companies            { get; set; }
    public DbSet<CompanyAlias>     CompanyAliases       { get; set; }
    public DbSet<Feature>          Features             { get; set; }
    public DbSet<FeatureAlias>     FeatureAliases       { get; set; }
    public DbSet<Series>           Series               { get; set; }
    public DbSet<Platform>         Platforms            { get; set; }
    public DbSet<PlatformAlias>    PlatformAliases      { get; set; }
    public DbSet<Tag>              Tags                 { get; set; }
    public DbSet<TagAlias>         TagAliases           { get; set; }
    public DbSet<Category>         Categories           { get; set; }
    public DbSet<Genre>            Genres               { get; set; }
    public DbSet<GenreAlias>       GenreAliases         { get; set; }
    public DbSet<Filter>           Filters              { get; set; }
    public DbSet<Operation>        Operations           { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
}