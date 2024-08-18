using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<VertexEntity> VertexEntities { get; set; }
    public DbSet<EdgeEntity> EdgeEntities { get; set; }
    public DbSet<VertexAttribute> VertexAttributes { get; set; }
    public DbSet<EdgeAttribute> EdgeAttributes { get; set; }
    public DbSet<VertexValue> VertexValues { get; set; }
    public DbSet<EdgeValue> EdgeValues { get; set; }
    public DbSet<DataSet> DataSets { get; set; }
    
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        base.OnModelCreating(modelBuilder);
    }
}