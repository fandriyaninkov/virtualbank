using Microsoft.EntityFrameworkCore;

namespace IdentityService.DB;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var user = modelBuilder.Entity<User>();
        user.ToTable("users");
        user.HasKey(p => p.Id);

        user.Property(x => x.Id).HasColumnName("id");
        user.Property(x => x.CreatedAt).HasColumnName("createdat");
        user.Property(x => x.Email).HasColumnName("email");
        user.Property(x => x.PasswordHash).HasColumnName("passwordhash");
    }
}
