
using BookLib;
using BookLib.Data;
using BookLib.Services;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookCatalogContext>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddTransient<IBookService, BookService>();

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

using var keepAliveConnection = new SqliteConnection(BookCatalogContext.ConnectionString);
keepAliveConnection.Open();

BookCatalogContext.PopulateDb();
await BookCatalogContext.WriteBookEntityToConsole("Pan Tadeusz");

app.Run();


