using CookBook.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Data;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {

    }
    public DbSet<Recipe> Recipes { get; set; }
}