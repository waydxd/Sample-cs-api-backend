namespace TodoApi.Models.Entities;

public class BookItem
{
    public long Id { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public long Category { get; set; }

    public DateTime PublishDate { get; set; }
    public string Name { get; set; } = string.Empty;
    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
