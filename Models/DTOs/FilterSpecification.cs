namespace TodoApi.Models.DTOs;

/// <summary>Logical operators for combining multiple filters or conditions.</summary>
public enum FilterOperator
{
    /// <summary>All nested filters/conditions must match (logical AND).</summary>
    And,
    /// <summary>At least one nested filter/condition must match (logical OR).</summary>
    Or,
    /// <summary>Negates the combined result of nested filters/conditions (logical NOT).</summary>
    Not
}

/// <summary>Comparison operators for individual filter conditions.</summary>
public enum ComparisonOperator
{
    /// <summary>Property equals the value.</summary>
    Equals,
    /// <summary>Property does not equal the value.</summary>
    NotEquals,
    /// <summary>String property contains the value.</summary>
    Contains,
    /// <summary>Property is greater than the value.</summary>
    GreaterThan,
    /// <summary>Property is greater than or equal to the value.</summary>
    GreaterThanOrEqual,
    /// <summary>Property is less than the value.</summary>
    LessThan,
    /// <summary>Property is less than or equal to the value.</summary>
    LessThanOrEqual
}

/// <summary>
/// Represents a node in a dynamic filter tree. Each node has a logical operator,
/// optional nested sub-filters, and optional leaf conditions.
/// </summary>
/// <param name="Operator">Logical operator applied to this node's children.</param>
/// <param name="Filters">Nested sub-filter specifications.</param>
/// <param name="Conditions">Leaf-level conditions on specific fields.</param>
public record FilterSpecification(
    FilterOperator Operator,
    List<FilterSpecification>? Filters,
    List<Condition>? Conditions
);

/// <summary>A single field-level condition used within a FilterSpecification tree.</summary>
/// <param name="Field">The property name on the entity to evaluate.</param>
/// <param name="Operator">The comparison operator to apply.</param>
/// <param name="Value">The value to compare against (string representation).</param>
public record Condition(
    string Field,
    ComparisonOperator Operator,
    string? Value
);
