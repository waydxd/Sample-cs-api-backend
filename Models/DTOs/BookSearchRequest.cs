namespace TodoApi.Models.DTOs;

public record BookSearchRequest(
    string? CountryCode,
    DateTime? PublishDateNotLaterThan,
    DateTime? PublishDateNotEarlierThan,
    long? Category,
    string? Name,
    FilterSpecification? Filter
);
