using Server.Models.Database;

namespace Server.Database.Configuration;

public class SeriesConfiguration() : CollectionLabelConfiguration<Series, Game>(g => g.Series);
