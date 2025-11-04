using Microsoft.EntityFrameworkCore;

using ApiEcommerce.Models.Entities;
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {


    }
    
    

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
//        public DbSet<Test> Tests { get; set; }
    //public DbSet<User> Users { get; set; }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }



    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);

    //     //Insertar datos iniciales en la tabla Categories
    //     modelBuilder.Entity<Category>().HasData(
    //         new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
    //         new Category { Id = 2, Name = "Books", Description = "Various kinds of books" },
    //         new Category { Id = 3, Name = "Clothing", Description = "Apparel and accessories" }
    //     );

        
    // }
}
