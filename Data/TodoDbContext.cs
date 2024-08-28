using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models.Auth;
using TodoApp.Models.Todos;

namespace TodoApp.Data
{
    public class TodoDbContext : IdentityDbContext<UserAccountModel, IdentityRole<Guid>, Guid>
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        { }

        public DbSet<Todo> Todos { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Todo>(todo =>
            {
                todo.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                todo.HasOne<UserAccountModel>()
                    .WithMany()
                    .HasForeignKey(t => t.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder
                .Entity<Status>()
                .HasData(
                    new Status { StatusId = "new", Name = "New" },
                    new Status { StatusId = "started", Name = "Started" },
                    new Status { StatusId = "finished", Name = "Finished" }
                );
        }
    }
}