namespace Server.Models.Database;

public class Filter : NamedDatabaseObject
{
    public List<Guid> PreFilterIds { get; set; } = [];
    public string Definition { get; set; }
    public bool Visible { get;set; }
    public bool Deleted { get; set; }

    public ICollection<Filter> PreFilters { get; set; } = [];
}

public class FilterDefinition : NamedDatabaseObject
{
    public List<Conditions> Conditions { get; set; } = [];
    public List<Sort> SortBy { get; set; } = [];
    public Grouping? GroupBy { get; set; }
}


public class Conditions {
    public string Field { get; set; }
    public FilterOperator Operator { get; set; }
    public object? Value { get; set; }
    public string? Type { get; set; }
    public bool? CaseSensitive { get; set; }
}

public class Grouping {
    public LogicOperator Logic { get; set; }
    public List<Conditions> Conditions { get; set; } = [];
}

public enum LogicOperator { And, Or }
public enum FilterOperator {
    Equal, NotEqual,
    GreaterThan, GreaterThanOrEqual,
    LessThan, LessThanOrEqual,
    Contains, StartsWith, EndsWith,
    In, Between, IsNull, NotNull
}

public class Sort {
    public string Field { get; set; }
    public bool Ascending { get; set; }
}