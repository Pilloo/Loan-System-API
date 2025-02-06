using Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AuthDbContext : IdentityDbContext<User>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public override DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Ignore(x => x.PhoneNumber);
        modelBuilder.Entity<User>().Ignore(x => x.PhoneNumberConfirmed);
        modelBuilder.Entity<User>().Ignore(x => x.TwoFactorEnabled);
        modelBuilder.Entity<User>().Ignore(x => x.LockoutEnabled);
        modelBuilder.Entity<User>().Ignore(x => x.LockoutEnd);
    }
}