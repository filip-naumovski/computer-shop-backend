using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerShopBackend.Models;
using ComputerShopBackend.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ComputerShopBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.User)]
    public class OrdersController : ControllerBase
    {
        private readonly ComputerShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ComputerShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var user = await _userManager.FindByNameAsync(username);
            var roles = await _userManager.GetRolesAsync(user);
            if(roles.Contains(UserRoles.Admin))
            {
                return await _context.Orders.Include(o => o.Products).Include(o => o.ApplicationUser).ToListAsync();
            } else
            {
                return await _context.Orders.Include(o => o.Products).Include(o => o.ApplicationUser).Where(o => o.UserId == user.Id).ToListAsync();
            }
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            // TODO: Check if user tries to access Orders that are not his.
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            order.Accepted = true;
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder()
        {
            var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var user = await _userManager.FindByNameAsync(username);
            Console.WriteLine("username: " + username);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
            var products = await _context.Products.Where(p => p.CartId == cart.Id).ToListAsync();
            var order = new Order()
            {
                Products = products,
                ApplicationUser = user,
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            foreach (var product in products)
            {
                product.Order = order;
                _context.Entry(product).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            cart.Products.Clear();
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
