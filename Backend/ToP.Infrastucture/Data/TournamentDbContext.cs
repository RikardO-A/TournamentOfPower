using Microsoft.EntityFrameworkCore;
using ToP.Domain.Classes;

namespace ToP.Infrastructure.Data
{
    public class TournamentDbContext : DbContext
    {
        public TournamentDbContext(DbContextOptions<TournamentDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Player entity
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(p => p.Image)
                    .HasMaxLength(500);
                
                // Create index for faster lookups
                entity.HasIndex(p => p.Name);
            });

            // Seed initial data (same as from CiP-02)
            modelBuilder.Entity<Player>().HasData(
                new Player { Id = 1, Name = "Alice", Image = "" },
                new Player { Id = 2, Name = "Bob", Image = "" },
                new Player { Id = 3, Name = "Charlie", Image = "" },
                new Player { Id = 4, Name = "Diana", Image = "" },
                new Player { Id = 5, Name = "Ethan", Image = "" },
                new Player { Id = 6, Name = "Fiona", Image = "" },
                new Player { Id = 7, Name = "George", Image = "" },
                new Player { Id = 8, Name = "Hannah", Image = "" },
                new Player { Id = 9, Name = "Isaac", Image = "" },
                new Player { Id = 10, Name = "Julia", Image = "" },
                new Player { Id = 11, Name = "Kevin", Image = "" },
                new Player { Id = 12, Name = "Laura", Image = "" },
                new Player { Id = 13, Name = "Michael", Image = "" },
                new Player { Id = 14, Name = "Nina", Image = "" },
                new Player { Id = 15, Name = "Oscar", Image = "" },
                new Player { Id = 16, Name = "Paula", Image = "" },
                new Player { Id = 17, Name = "Quentin", Image = "" },
                new Player { Id = 18, Name = "Rachel", Image = "" },
                new Player { Id = 19, Name = "Samuel", Image = "" },
                new Player { Id = 20, Name = "Tina", Image = "" }
            );
        }
    }
}
