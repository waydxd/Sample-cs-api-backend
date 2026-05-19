using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Extensions;
using TodoApi.Models.Entities;

namespace TodoApi.Endpoints;

/// <summary>Minimal API module for managing to-do items under the /api/todos route.</summary>
public class TodoEndpoints : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/todos").WithTags("Todo Management");

        group.MapGet("/", GetTodosAsync);
        group.MapPost("/", CreateTodoAsync);
    }

    /// <summary>Returns all to-do items.</summary>
    private static async Task<IResult> GetTodosAsync(AppDbContext db)
    {
        var todos = await db.Todos.AsNoTracking().ToListAsync();
        return Results.Ok(todos);
    }

    /// <summary>Creates a new to-do item. Requires a non-empty title.</summary>
    private static async Task<IResult> CreateTodoAsync(TodoItem todo, AppDbContext db)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            return Results.BadRequest(new { message = "Title cannot be empty" });
        }

        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return Results.Created($"/todos/{todo.Id}", todo);
    }
}
