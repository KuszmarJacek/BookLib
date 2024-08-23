using BookLib.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLib.Data
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        protected readonly IConfiguration _config;
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public RepositoryContext(DbContextOptions options) : base(options) { }
        public RepositoryContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            new DbInitializer(builder).Seed();
        }
    }
}
