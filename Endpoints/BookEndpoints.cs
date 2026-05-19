using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Extensions;
using TodoApi.Models.DTOs;
using TodoApi.Models.Entities;
using FluentValidation;
namespace TodoApi.Endpoints;

public class BookEndpoints : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/books").WithTags("Book Management");

        group.MapGet("/", GetBooksAsync);
        group.MapPost("/", CreateBookAsync);
    }

    private static async Task<IResult> GetBooksAsync(AppDbContext db)
    {
        var books = await db.Todos.AsNoTracking().ToListAsync();
        return Results.Ok(books);
    }

    private static async Task<IResult> CreateBookAsync(
        CreateBookDto dto, 
        IValidator<CreateBookDto> validator, // Injected automatically
        AppDbContext db)
    {
        // 1. Run Validation
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            // Returns a structured 400 Bad Request with all error messages
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // 2. Map DTO to Entity if validation passes
        var book = new BookItem
        {
            CountryCode = dto.CountryCode,
            Category = dto.Category,
            PublishDate = dto.PublishDate,
            Name = dto.Name
        };

        db.Books.Add(book);
        await db.SaveChangesAsync();
        
        return Results.Created($"/books/{book.Id}", book);
    }

        private static async Task<IResult> UpdateBookAsync(
        CreateBookDto dto, 
        IValidator<CreateBookDto> validator, // Injected automatically
        AppDbContext db)
    {
        // 1. Run Validation
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            // Returns a structured 400 Bad Request with all error messages
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // 2. Map DTO to Entity if validation passes
        var book = new BookItem
        {
            CountryCode = dto.CountryCode,
            Category = dto.Category,
            PublishDate = dto.PublishDate,
            Name = dto.Name
        };

        db.Books.Add(book);
        await db.SaveChangesAsync();
        
        return Results.Created($"/books/{book.Id}", book);
    }
}
