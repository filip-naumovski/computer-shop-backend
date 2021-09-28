using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerShopBackend.Models;
using ComputerShopBackend.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ComputerShopBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
    public class CartsController : ControllerBase
    {
        private readonly ComputerShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CartsController(ComputerShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCarts()
        {
            var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var user = await _userManager.FindByNameAsync(username);
            return await _context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.UserId == user.Id);
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutCart(long id, Cart cart)
        {
            
            if (id != cart.Id)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // PUT: api/Carts
        // Update cart with new array of products

        [HttpPut]
        public async Task<IActionResult> PutCart(Product[] products)
        {
            var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var user = await _userManager.FindByNameAsync(username);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
            cart.Products = products;
            foreach(Product product in products)
            {
                product.Cart = cart;
                _context.Entry(product).State = EntityState.Modified;
            }
            
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Carts
        // Add Product to cart

        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Product product)
        {
            var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var user = await _userManager.FindByNameAsync(username);
            Console.WriteLine("User:" + user.UserName);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
            Console.WriteLine("Cart for user:" + cart.ApplicationUser.UserName);
            product.Cart = cart;
            _context.Entry(product).State = EntityState.Modified;
            cart.Products.Add(product);
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCart", new { id = cart.Id }, cart);
        }
    }
}
