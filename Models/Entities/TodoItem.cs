namespace TodoApi.Models.Entities;

/// <summary>Represents a to-do item in the Todos table.</summary>
public class TodoItem
{
    /// <summary>Unique identifier for the to-do item.</summary>
    public long Id { get; set; }

    /// <summary>Title or description of the to-do item.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Indicates whether the to-do item has been completed.</summary>
    public bool IsCompleted { get; set; }

    /// <summary>UTC timestamp when the to-do item was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
