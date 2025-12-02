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

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(p => p.Image)
                    .HasMaxLength(500);
                entity.HasIndex(p => p.Name);
            });

            modelBuilder.Entity<Player>().HasData(
                new Player { Id = 1, Name = "Alice", Image = "/images/players/alice.png" },
                new Player { Id = 2, Name = "Bob", Image = "/images/players/bob.png" },
                new Player { Id = 3, Name = "Charlie", Image = "/images/players/charlie.png" },
                new Player { Id = 4, Name = "Diana", Image = "/images/players/diana.png" },
                new Player { Id = 5, Name = "Ethan", Image = "/images/players/ethan.png" },
                new Player { Id = 6, Name = "Fiona", Image = "/images/players/fiona.png" },
                new Player { Id = 7, Name = "George", Image = "/images/players/george.png" },
                new Player { Id = 8, Name = "Hannah", Image = "/images/players/hannah.png" },
                new Player { Id = 9, Name = "Isaac", Image = "/images/players/isaac.png" },
                new Player { Id = 10, Name = "Julia", Image = "/images/players/julia.png" },
                new Player { Id = 11, Name = "Kevin", Image = "/images/players/kevin.png" },
                new Player { Id = 12, Name = "Laura", Image = "/images/players/laura.png" },
                new Player { Id = 13, Name = "Michael", Image = "/images/players/michael.png" },
                new Player { Id = 14, Name = "Nina", Image = "/images/players/nina.png" },
                new Player { Id = 15, Name = "Oscar", Image = "/images/players/oscar.png" },
                new Player { Id = 16, Name = "Paula", Image = "/images/players/paula.png" },
                new Player { Id = 17, Name = "Quentin", Image = "/images/players/quentin.png" },
                new Player { Id = 18, Name = "Rachel", Image = "/images/players/rachel.png" },
                new Player { Id = 19, Name = "Samuel", Image = "/images/players/samuel.png" },
                new Player { Id = 20, Name = "Tina", Image = "/images/players/tina.png" }
            );
        }
    }
}
