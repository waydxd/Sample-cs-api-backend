using System.Linq.Expressions;
using TodoApi.Models.DTOs;

namespace TodoApi.Extensions;

public static class FilterExpressionBuilder
{
    public static Expression<Func<T, bool>> Build<T>(FilterSpecification spec)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = BuildSpec(spec, param);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    private static Expression BuildSpec(FilterSpecification spec, Expression param)
    {
        var parts = new List<Expression>();

        if (spec.Conditions is not null)
        {
            foreach (var c in spec.Conditions)
            {
                parts.Add(BuildCondition(c, param));
            }
        }

        if (spec.Filters is not null)
        {
            foreach (var f in spec.Filters)
            {
                parts.Add(BuildSpec(f, param));
            }
        }

        if (parts.Count == 0)
            return Expression.Constant(true);

        Expression combined = parts.Aggregate(
            spec.Operator == FilterOperator.Or
                ? (Expression left, Expression right) => Expression.OrElse(left, right)
                : (Expression left, Expression right) => Expression.AndAlso(left, right)
        );

        return spec.Operator == FilterOperator.Not
            ? Expression.Not(combined)
            : combined;
    }

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

    private static object? ConvertValue(Type targetType, string? value)
    {
        if (value is null) return null;
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(long)) return long.Parse(value);
        if (targetType == typeof(DateTime)) return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
        return Convert.ChangeType(value, targetType);
    }
}
