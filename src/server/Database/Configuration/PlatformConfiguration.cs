using Server.Models.Database;

namespace Server.Database.Configuration;

public class PlatformConfiguration() : CollectionLabelConfiguration<Platform, GameVariant>(g => g.Platforms);
