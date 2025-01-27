using CookBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Data;

public class ApplicationDBContext : IdentityDbContext<AppUser>
{
    public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {

    }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
            },
            new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "USER",
            },
            new IdentityRole
            {
                Id = "3",
                Name = "Pending",
                NormalizedName = "PENDING",
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }
}
