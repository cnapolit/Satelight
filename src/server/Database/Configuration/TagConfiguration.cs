using Server.Models.Database;

namespace Server.Database.Configuration;

public class TagConfiguration() : CollectionLabelConfiguration<Tag, Game>(g => g.Tags);