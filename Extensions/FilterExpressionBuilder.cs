using System.Linq.Expressions;
using TodoApi.Models.DTOs;

namespace TodoApi.Extensions;

/// <summary>
/// Builds LINQ <see cref="Expression{TDelegate}"/> trees at runtime from a
/// <see cref="FilterSpecification"/> tree, enabling dynamic query filtering.
/// </summary>
public static class FilterExpressionBuilder
{
    /// <summary>
    /// Builds an <see cref="Expression{TDelegate}"/> that can be passed to
    /// <c>IQueryable.Where()</c> based on the given filter specification.
    /// </summary>
    /// <typeparam name="T">The entity type to filter.</typeparam>
    /// <param name="spec">The filter specification tree.</param>
    /// <returns>An expression predicate representing the filter logic.</returns>
    public static Expression<Func<T, bool>> Build<T>(FilterSpecification spec)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = BuildSpec(spec, param);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>
    /// Recursively builds an expression tree from a <see cref="FilterSpecification"/> node.
    /// Combines all child conditions and sub-filters using the node's operator,
    /// then optionally wraps the result in a logical NOT.
    /// </summary>
    private static Expression BuildSpec(FilterSpecification spec, Expression param)
    {
        var parts = new List<Expression>();

        // Add leaf-level conditions, e.g. "Name Contains foo".
        if (spec.Conditions is not null)
        {
            foreach (var c in spec.Conditions)
            {
                parts.Add(BuildCondition(c, param));
            }
        }

        // Add nested sub-filter nodes recursively.
        if (spec.Filters is not null)
        {
            foreach (var f in spec.Filters)
            {
                parts.Add(BuildSpec(f, param));
            }
        }

        // If no children exist, return a no-op (matches everything).
        if (parts.Count == 0)
            return Expression.Constant(true);

        // Combine all child expressions using AND or OR as specified.
        Expression combined = parts.Aggregate(
            spec.Operator == FilterOperator.Or
                ? (Expression left, Expression right) => Expression.OrElse(left, right)
                : (Expression left, Expression right) => Expression.AndAlso(left, right)
        );

        // Wrap the combined result in NOT if the operator requires it.
        return spec.Operator == FilterOperator.Not
            ? Expression.Not(combined)
            : combined;
    }

    /// <summary>
    /// Builds a single comparison expression (e.g., property == value, property.Contains(value))
    /// from a <see cref="Condition"/>.
    /// </summary>
    private static Expression BuildCondition(Condition condition, Expression param)
    {
        var property = Expression.Property(param, condition.Field);
        var propType = property.Type;

        Expression constant = condition.Value is null
            ? Expression.Constant(null, propType)
            : Expression.Constant(ConvertValue(propType, condition.Value), propType);

        return condition.Operator switch
        {
            ComparisonOperator.Equals => Expression.Equal(property, constant),
            ComparisonOperator.NotEquals => Expression.NotEqual(property, constant),
            ComparisonOperator.Contains when propType == typeof(string)
                => Expression.Call(property, "Contains", Type.EmptyTypes, constant),
            ComparisonOperator.GreaterThan => Expression.GreaterThan(property, constant),
            ComparisonOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
            ComparisonOperator.LessThan => Expression.LessThan(property, constant),
            ComparisonOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
            _ => Expression.Equal(property, constant)
        };
    }

    /// <summary>
    /// Converts a string value to the target type. Supports string, long, DateTime,
    /// and falls back to <see cref="Convert.ChangeType(object, Type)"/> for other types.
    /// </summary>
    private static object? ConvertValue(Type targetType, string? value)
    {
        if (value is null) return null;
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(long)) return long.Parse(value);
        if (targetType == typeof(DateTime)) return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
        return Convert.ChangeType(value, targetType);
    }
}
