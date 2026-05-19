namespace TodoApi.Models.DTOs;

public record UpdateBookDto(
    string CountryCode,
    long Category,
    DateTime PublishDate,
    string Name
);
