using BookLib.ActionFilters;
using BookLib.Contracts;
using BookLib.Data;
using BookLib.Domain.Repositories;
using BookLib.Extensions;
using BookLib.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using BookLib.Logging;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<RepositoryContext>(options =>
{
    var sqliteConnStr = config.GetConnectionString("sqlite-conn-string");
    options.UseSqlite(sqliteConnStr);
});

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

using var keepAliveConnection = new SqliteConnection(config.GetConnectionString("sqlite-conn-string"));
keepAliveConnection.Open();

app.Run();


