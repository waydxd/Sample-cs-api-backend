namespace TodoApi.Models.DTOs;

/// <summary>Data transfer object for creating a new book.</summary>
/// <param name="CountryCode">ISO country code (2-3 uppercase letters).</param>
/// <param name="Category">Category identifier (must be positive).</param>
/// <param name="PublishDate">Publication date (must not be in the future).</param>
/// <param name="Name">Book name (max 200 characters, cannot be all digits).</param>
public record CreateBookDto(
    string CountryCode,
    long Category,
    DateTime PublishDate,
    string Name
);