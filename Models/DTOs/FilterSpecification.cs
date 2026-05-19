namespace TodoApi.Models.DTOs;

public enum FilterOperator
{
    And,
    Or,
    Not
}

public enum ComparisonOperator
{
    Equals,
    NotEquals,
    Contains,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual
}

public record FilterSpecification(
    FilterOperator Operator,
    List<FilterSpecification>? Filters,
    List<Condition>? Conditions
);

public record Condition(
    string Field,
    ComparisonOperator Operator,
    string? Value
);
