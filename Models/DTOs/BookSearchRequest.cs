namespace TodoApi.Models.DTOs;

/// <summary>
/// Search request for querying books. Supports flat query-string parameters
/// and a nested FilterSpecification tree for advanced dynamic filtering.
/// </summary>
/// <param name="CountryCode">Optional ISO country code filter (2-3 uppercase letters).</param>
/// <param name="PublishDateNotLaterThan">Optional upper bound for publish date (inclusive).</param>
/// <param name="PublishDateNotEarlierThan">Optional lower bound for publish date (inclusive).</param>
/// <param name="Category">Optional category filter.</param>
/// <param name="Name">Optional name substring filter.</param>
/// <param name="Filter">Optional dynamic filter tree for complex queries.</param>
public record BookSearchRequest(
    string? CountryCode,
    DateTime? PublishDateNotLaterThan,
    DateTime? PublishDateNotEarlierThan,
    long? Category,
    string? Name,
    FilterSpecification? Filter
);
