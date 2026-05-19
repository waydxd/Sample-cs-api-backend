namespace TodoApi.Models.Entities;

/// <summary>Represents a book in the Books table.</summary>
public class BookItem
{
    /// <summary>Unique identifier for the book.</summary>
    public long Id { get; set; }

    /// <summary>ISO country code (e.g., US, HK, GBR).</summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>Category identifier for the book.</summary>
    public long Category { get; set; }

    /// <summary>Publication date of the book.</summary>
    public DateTime PublishDate { get; set; }

    /// <summary>Name or title of the book.</summary>
    public string Name { get; set; } = string.Empty;
}
