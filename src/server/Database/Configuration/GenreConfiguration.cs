using Server.Models.Database;

namespace Server.Database.Configuration;

public class GenreConfiguration() : CollectionLabelConfiguration<Genre, Game>(g => g.Genres);
