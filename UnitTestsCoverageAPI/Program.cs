var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var books = new List<Book>
{
    new Book { Id = 1, Title = "Steve Jobs", Author = "Walter Isaacson"},
    new Book { Id = 2, Title = "Napraw siê!", Author = "Gary John Bishop"},
    new Book { Id = 3, Title = "Dzieci z Bullerbyn", Author = "Astrid Lindgren"},
    new Book { Id = 4, Title = "Ania z Zielonego Wzgórza", Author = "Lucy Maud Montgomery"},
};

app.MapGet("/book", () =>
{
    return books;
});

app.MapGet("book/id", (int id) =>
{
    var book = books.Find(b => b.Id == id);
    if (book is null)
        return Results.NotFound("Sorry this book doesn't exist");

    return Results.Ok(book);
});

app.MapPost("/book", (Book book) =>
{
    books.Add(book);
    return books;
});

app.MapPut("book/id", (Book updatedBook, int id) =>
{
    var book = books.Find(b => b.Id == id);
    if (book is null)
        return Results.NotFound("Sorry this book doesn't exist");

    book.Title = updatedBook.Title;
    book.Author = updatedBook.Author;

    return Results.Ok(book);
});

app.MapDelete("book/id", (int id) =>
{
    var book = books.Find(b => b.Id == id);
    if (book is null)
        return Results.NotFound("Sorry this book doesn't exist");

    books.Remove(book);

    return Results.Ok(book);
});

app.Run();

class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
}