using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Extensions;
using TodoApi.Models.Entities;

namespace TodoApi.Endpoints;

public class TodoEndpoints : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/todos").WithTags("Todo Management");

        group.MapGet("/", GetTodosAsync);
        group.MapPost("/", CreateTodoAsync);
    }

    private static async Task<IResult> GetTodosAsync(AppDbContext db)
    {
        var todos = await db.Todos.AsNoTracking().ToListAsync();
        return Results.Ok(todos);
    }

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
