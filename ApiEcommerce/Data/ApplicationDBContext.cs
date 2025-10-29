using Microsoft.EntityFrameworkCore;

using ApiEcommerce.Models.Entities;
class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
}
