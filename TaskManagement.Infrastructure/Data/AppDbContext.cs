using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Status).HasConversion<string>();
            entity.Property(t => t.Priority).HasConversion<string>();

            entity.HasOne(t => t.AssignedToUser)
                  .WithMany(u => u.AssignedTasks)
                  .HasForeignKey(t => t.AssignedToUserId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.Category)
                  .WithMany(c => c.Tasks)
                  .HasForeignKey(t => t.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        // Seed default categories
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "General",    Description = "General tasks" },
            new Category { Id = 2, Name = "Work",       Description = "Work related tasks" },
            new Category { Id = 3, Name = "Personal",   Description = "Personal tasks" },
            new Category { Id = 4, Name = "Urgent",     Description = "High priority tasks" }
        );
    }
}
