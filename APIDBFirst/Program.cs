using Microsoft.EntityFrameworkCore;
using CodeFirstEmptyController.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// -------------------------
// Configure DbContext
// -------------------------
builder.Services.AddDbContext<BookAuthContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------------
// Swagger / OpenAPI
// -------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------
// Middleware
// -------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// -------------------------
// WeatherForecast Endpoint
// -------------------------
var summaries = new[]
{
    "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
};

app.MapGet("/", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// -------------------------
// Authors Endpoints
// -------------------------
app.MapGet("/api/authors", async (BookAuthContext context) =>
    await context.Authors.ToListAsync());

app.MapGet("/api/authors/{id}", async (int id, BookAuthContext context) =>
{
    var author = await context.Authors.FindAsync(id);
    return author is not null ? Results.Ok(author) : Results.NotFound();
});

app.MapPost("/api/authors", async (Author? author, BookAuthContext context) =>
{
    if (author == null) return Results.BadRequest("No body provided");

    context.Authors.Add(author);
    await context.SaveChangesAsync();
    return Results.Created($"/api/authors/{author.AuthId}", author);
});

app.MapPut("/api/authors/{id}", async (int id, Author? author, BookAuthContext context) =>
{
    if (author == null) return Results.BadRequest("No body provided");

    var existing = await context.Authors.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.AuthName = author.AuthName;
    await context.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDelete("/api/authors/{id}", async (int id, BookAuthContext context) =>
{
    var author = await context.Authors.FindAsync(id);
    if (author is null) return Results.NotFound();

    context.Authors.Remove(author);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// -------------------------
// Books Endpoints
// -------------------------

app.MapGet("/api/books", async (BookAuthContext context) =>
{
    var books = await context.Books
        .Include(b => b.author)
        .Select(b => new 
        {
            b.BookId,
            b.Title,
            b.Price,
            PublicationYear = b.PublicationYear.ToString("yyyy-MM-dd"),
            b.AuthId,
            AuthorName = b.author != null ? b.author.AuthName : null
        })
        .ToListAsync();

    return Results.Ok(books);
});


app.MapGet("/api/books/{id}", async (int id, BookAuthContext context) =>
{
    var book = await context.Books
        .Include(b => b.author)
        .Where(b => b.BookId == id)
        .Select(b => new
        {
            b.BookId,
            b.Title,
            b.Price,
            PublicationYear = b.PublicationYear.ToString("yyyy-MM-dd"),
            b.AuthId,
            AuthorName = b.author != null ? b.author.AuthName : null
        })
        .FirstOrDefaultAsync();

    return book is not null ? Results.Ok(book) : Results.NotFound();
});

// POST book
app.MapPost("/api/books", async (Book? book, BookAuthContext context) =>
{
    if (book == null) return Results.BadRequest("No body provided");

    var authorExists = await context.Authors.AnyAsync(a => a.AuthId == book.AuthId);
    if (!authorExists)
        return Results.BadRequest($"Author with Id {book.AuthId} does not exist.");

    context.Books.Add(book);
    await context.SaveChangesAsync();
    return Results.Created($"/api/books/{book.BookId}", book);
});


// PUT book
app.MapPut("/api/books/{id}", async (int id, Book? book, BookAuthContext context) =>
{
    if (book == null) 
        return Results.BadRequest("No body provided");

    var existing = await context.Books.FindAsync(id);
    if (existing == null) 
        return Results.NotFound();

    var authorExists = await context.Authors.AnyAsync(a => a.AuthId == book.AuthId);
    if (!authorExists)
        return Results.BadRequest($"Author with Id {book.AuthId} does not exist.");

    existing.Title = book.Title;
    existing.Price = book.Price;
    existing.PublicationYear = book.PublicationYear;
    existing.AuthId = book.AuthId;

    await context.SaveChangesAsync();
    return Results.NoContent();
});


// DELETE book
app.MapDelete("/api/books/{id}", async (int id, BookAuthContext context) =>
{
    var book = await context.Books.FindAsync(id);
    if (book == null) return Results.NotFound();

    context.Books.Remove(book);
    await context.SaveChangesAsync();
    return Results.NoContent();
});
// -------------------------
app.Run();

// -------------------------
// WeatherForecast record
// -------------------------
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
