using Microsoft.EntityFrameworkCore;
using BookLib.Entities;

namespace BookLib.Data
{
    public class DbInitializer
    {
        private readonly ModelBuilder _builder;

        public DbInitializer(ModelBuilder builder)
        {
            _builder = builder;
        }
        public void Seed()
        {
            //var tag1 = new Tag { Id = 1, Name = "History"};
            //var tag2 = new Tag { Id = 2, Name = "Epic" };
            //var tag3 = new Tag { Id = 3, Name = "Poem" };
            //var rating1 = new BookRating { Id = 1, Score = 5, Comment = "Pogchamp" };
            //var rating2 = new BookRating { Id = 2, Score = 3, Comment = "Ok" };

            //_builder.Entity<Tag>().HasData(
            //    tag1, 
            //    tag2, 
            //    tag3);

            //_builder.Entity<BookRating>().HasData(
            //    rating1,
            //    rating2);

            _builder.Entity<Book>().HasData(
                //new Book { Id = 1, Title = "Pan Tadeusz", Authors = new List<Author>(new Author[] { new Author { Id = 1, FirstName = "Adam", LastName = "Mickiewicz" } })},
                new Book { Id = 2, Title = "Pride and Prejudice" },
                new Book { Id = 3, Title = "1984" },
                new Book { Id = 4, Title = "Crime and Punishment" },
                new Book { Id = 5, Title = "The Great Gatsby" });

        }
    }
}
