namespace TodoApi.Models.DTOs;

public record CreateBookDto(
    string CountryCode,
    long Category,
    DateTime PublishDate,
    string Name
);