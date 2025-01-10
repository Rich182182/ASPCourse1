
using Microsoft.EntityFrameworkCore;
using Rich.Models;

namespace Rich.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {   

        }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Horror", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Fantasy", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Historry", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Romance", DisplayOrder = 5 }
                );
        }
    }
}
