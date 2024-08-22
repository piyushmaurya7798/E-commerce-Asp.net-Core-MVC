using AddToCart.Models;
using Microsoft.EntityFrameworkCore;

namespace AddToCart.Context
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
