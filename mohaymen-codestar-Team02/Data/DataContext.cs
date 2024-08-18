using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Models;
using Attribute = mohaymen_codestar_Team02.Models.Attribute;

namespace mohaymen_codestar_Team02.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Entity> VertexEntities { get; set; }
    public DbSet<Entity> EdgeEntities { get; set; }
    public DbSet<Attribute> VertexAttributes { get; set; }
    public DbSet<Attribute> EdgeAttributes { get; set; }
    public DbSet<Value> VertexValues { get; set; }
    public DbSet<Value> EdgeValues { get; set; }
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