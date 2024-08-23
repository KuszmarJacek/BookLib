using BookLib.Data;
using BookLib.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<RepositoryContext>(options =>
{
    var sqliteConnStr = config.GetConnectionString("sqlite-conn-string");
    options.UseSqlite(sqliteConnStr);
});

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddTransient<IBookService, BookService>();

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

using var keepAliveConnection = new SqliteConnection(config.GetConnectionString("sqlite-conn-string"));
keepAliveConnection.Open();

//RepositoryContext.PopulateDb();
//await RepositoryContext.WriteBookEntityToConsole("Pan Tadeusz");

app.Run();


