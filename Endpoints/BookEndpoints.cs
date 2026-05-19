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
        group.MapPut("/{id:long}", UpdateBookAsync);
        group.MapDelete("/{id:long}", DeleteBookAsync);
    }

    private static async Task<IResult> GetBooksAsync(AppDbContext db)
    {
        var books = await db.Books.AsNoTracking().ToListAsync();
        return Results.Ok(books);
    }

    private static async Task<IResult> CreateBookAsync(
        CreateBookDto dto,
        IValidator<CreateBookDto> validator,
        AppDbContext db)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

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
        long id,
        UpdateBookDto dto,
        IValidator<UpdateBookDto> validator,
        AppDbContext db)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var book = await db.Books.FindAsync(id);
        if (book is null)
            return Results.NotFound();

        book.CountryCode = dto.CountryCode;
        book.Category = dto.Category;
        book.PublishDate = dto.PublishDate;
        book.Name = dto.Name;

        await db.SaveChangesAsync();

        return Results.Ok(book);
    }

    private static async Task<IResult> DeleteBookAsync(long id, AppDbContext db)
    {
        var book = await db.Books.FindAsync(id);
        if (book is null)
            return Results.NotFound();

        db.Books.Remove(book);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
