using Microsoft.EntityFrameworkCore;
using VismaAPI.Domain;

namespace VismaAPI.Data;

public class VismaAPIContext : DbContext
{
    public VismaAPIContext(DbContextOptions<VismaAPIContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(e => e.Boss)
                  .WithMany()
                  .HasForeignKey(e => e.BossId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
