using System.Text.Json;
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
        group.MapGet("/search", SearchBooksGetAsync);
        group.MapPost("/search", SearchBooksPostAsync);
        group.MapPost("/", CreateBookAsync);
        group.MapGet("/{id:long}", GetBookByIdAsync);
        group.MapPut("/{id:long}", UpdateBookAsync);
        group.MapDelete("/{id:long}", DeleteBookAsync);
    }

    private static async Task<IResult> GetBooksAsync(AppDbContext db)
    {
        var books = await db.Books.AsNoTracking().ToListAsync();
        return Results.Ok(books);
    }
    private static async Task<IResult> GetBookByIdAsync(long id, AppDbContext db)
{
    var book = await db.Books.FindAsync(id);

    return book is not null 
        ? Results.Ok(book) 
        : Results.NotFound($"Book with ID {id} not found.");
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

    private static async Task<IResult> SearchBooksGetAsync(
        string? countryCode,
        DateTime? publishDateNotLaterThan,
        DateTime? publishDateNotEarlierThan,
        long? category,
        string? name,
        AppDbContext db)
    {
        var request = new BookSearchRequest(countryCode, publishDateNotLaterThan, publishDateNotEarlierThan, category, name, null);
        var query = ApplySearchFilters(db.Books.AsNoTracking(), request);
        var results = await query.ToListAsync();
        return Results.Ok(results);
    }

    private static async Task<IResult> SearchBooksPostAsync(
        HttpContext httpContext,
        AppDbContext db)
    {
        BookSearchRequest? request;
        try
        {
            request = await httpContext.Request.ReadFromJsonAsync<BookSearchRequest>();
        }
        catch (JsonException ex)
        {
            await Console.Error.WriteLineAsync($"[JSON Error] {ex.Message}");
            return Results.Problem(
                detail: ex.InnerException?.Message ?? ex.Message,
                statusCode: 400,
                title: "Invalid JSON");
        }

        if (request is null)
            return Results.BadRequest(new { detail = "Request body cannot be empty." });

        var validator = httpContext.RequestServices.GetRequiredService<IValidator<BookSearchRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var query = ApplySearchFilters(db.Books.AsNoTracking(), request);

        if (request.Filter is not null)
            query = query.Where(FilterExpressionBuilder.Build<BookItem>(request.Filter));

        var results = await query.ToListAsync();
        return Results.Ok(results);
    }

    private static IQueryable<BookItem> ApplySearchFilters(IQueryable<BookItem> query, BookSearchRequest request)
    {
        if (request.CountryCode is not null)
            query = query.Where(b => b.CountryCode == request.CountryCode);

        if (request.PublishDateNotLaterThan is not null)
            query = query.Where(b => b.PublishDate <= request.PublishDateNotLaterThan.Value);

        if (request.PublishDateNotEarlierThan is not null)
            query = query.Where(b => b.PublishDate >= request.PublishDateNotEarlierThan.Value);

        if (request.Category is not null)
            query = query.Where(b => b.Category == request.Category.Value);

        if (request.Name is not null)
            query = query.Where(b => b.Name.Contains(request.Name));

        return query;
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
