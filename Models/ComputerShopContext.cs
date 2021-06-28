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
    }
}
