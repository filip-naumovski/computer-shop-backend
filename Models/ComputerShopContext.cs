using ComputerShopBackend.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerShopBackend.Models
{
    public class ComputerShopContext : IdentityDbContext<ApplicationUser>
    {
        public ComputerShopContext(DbContextOptions<ComputerShopContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(p => p.Cart)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<Order>()
                .HasOne<ApplicationUser>(b => b.ApplicationUser)
                .WithMany(a => a.Orders)
                .HasForeignKey(b => b.UserId);
            /*modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.Orders)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey(o => o.UserId);*/

            /*modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.ApplicationUser);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ApplicationUser)
                .WithMany(u => u.Orders);*/
            modelBuilder.Entity<Product>()
                .HasOne<Cart>(p => p.Cart)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CartId);
            modelBuilder.Entity<Product>()
                .HasOne<Order>(p => p.Order)
                .WithMany(o => o.Products)
                .HasForeignKey(p => p.OrderId);
        }
    }
}
