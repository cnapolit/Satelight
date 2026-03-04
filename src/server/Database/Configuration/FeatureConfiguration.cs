using Server.Models.Database;

namespace Server.Database.Configuration;

public class FeatureConfiguration() : CollectionLabelConfiguration<Feature, GameVariant>(g => g.Features);