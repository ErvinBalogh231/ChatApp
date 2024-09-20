using ChatServer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Database.Data
{
    internal class ChatServerContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;" +
                "Initial Catalog=msdb;" +
                "Persist Security Info=True;" +
                "User ID=sa;Password=Contrasea12345678!;" +
                "Encrypt=True;" +
                "Trust Server Certificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.Username)
                .HasPrincipalKey(e => e.UserName);
        }
    }
}
