using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Models;

namespace CorporateCMS.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Page> Pages { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Page entity configuration
        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.HasIndex(p => p.IsHomePage);
            entity.Property(p => p.Content).HasColumnType("TEXT");
        });
        
        // Menu entity configuration
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasOne(m => m.Parent)
                  .WithMany(m => m.Children)
                  .HasForeignKey(m => m.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(m => m.Order);
        });
        
        // Slider entity configuration
        modelBuilder.Entity<Slider>(entity =>
        {
            entity.HasIndex(s => s.Order);
        });
        
        // Announcement entity configuration
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasIndex(a => a.CreatedDate);
            entity.HasIndex(a => a.IsPinned);
            entity.Property(a => a.Content).HasColumnType("TEXT");
        });
    }
}
