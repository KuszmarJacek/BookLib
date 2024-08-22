using BookLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLib
{
    public class BookCatalogContext : DbContext
    {
        public const string ConnectionString = "DataSource=booklib.db;mode=memory;cache=shared";
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
        public static void PopulateDb()
        {
            using var dbContext = new BookCatalogContext();
            if (!dbContext.Database.EnsureCreated())
            {
                Console.WriteLine("Database already exists");
                return;
            }

            var panTadeuszBook = new Book { Title = "Pan Tadeusz" };
            panTadeuszBook.Ratings!.Add(new BookRating { Score = 5, Comment = "Pogchamp" });
            panTadeuszBook.Ratings!.Add(new BookRating { Score = 3, Comment = "Ok" });
            panTadeuszBook.Tags!.Add(new Tag { Name = "History" });
            panTadeuszBook.Tags!.Add(new Tag { Name = "Epic" });
            panTadeuszBook.Tags!.Add(new Tag { Name = "Poem" });
            panTadeuszBook.Authors!.Add(new Author { FirstName = "Adam", LastName = "Mickiewicz" });
            dbContext.Add(panTadeuszBook);

            dbContext.Add(new Book { Title = "Pride and Prejudice" });
            dbContext.Add(new Book { Title = "1984" });
            dbContext.Add(new Book { Title = "Crime and Punishment" });
            dbContext.Add(new Book { Title = "The Great Gatsby" });

            dbContext.SaveChanges();
        }

        public static async Task WriteBookEntityToConsole(string title)
        {
            using var dbContext = new BookCatalogContext();
            var book = await dbContext.Books.Include(book => book.Ratings).Include(book => book.Authors).Include(book => book.Tags).FirstOrDefaultAsync(book => book.Title == title);

            if (book == null)
            {
                Console.WriteLine($"Book with title {title} is not present in the database");
                return;
            }

            Console.WriteLine($"Book {book.Title} by {book.Authors.ToString()} with an id of: {book.Id} and its tags are:");
            book.Tags?.ForEach(tag => Console.WriteLine($"{tag.Name}"));
            Console.WriteLine($"Reviews of {book.Title} look like this:");
            book.Ratings?.ForEach(rating => Console.WriteLine($"comment: {rating.Comment}, score: {rating.Score}"));
        }
    }
}
