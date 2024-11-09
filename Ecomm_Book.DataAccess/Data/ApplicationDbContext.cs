using Ecomm_Book.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecomm_Book.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<MyProduct> MyProducts { get; set; }
        public DbSet<Company> Companies { get; set; }   
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<MyOrderHeader> MyOrderHeaders { get; set; }
        public DbSet<MyOrderDetail> MyOrderDetails { get; set; }
        public DbSet<MyShoppingCart> MyShoppingCarts { get; set;}
        public DbSet<AddressAnother>AddressAnothers { get; set;}
    }
}