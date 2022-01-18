using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingAPI.Models;

namespace ShoppingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;


        public CartsController(AppDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts.ToListAsync();
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
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
        }

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostCart(List<Product> cart)
        {
            var code = "";
            Cart c = _context.Carts.OrderByDescending(a => a.Id).FirstOrDefault();
            if (c != null && c.OrderId != "001")
            {
                string[] th = c.OrderId.Split('-');
                var result = Regex.Match(th[0], @"\d+").Value;

                if (result == DateTime.Today.ToString("yyyyMMdd"))
                {
                    int idd = Int32.Parse(th[1]);
                    idd++;
                    code = th[0] + '-' + idd.ToString("D3");
                }
                else
                {
                    code = "Inv" + DateTime.Today.ToString("yyyyMMdd") + '-' + "001";
                }
            }
            else
            {
                code = "Inv" + DateTime.Today.ToString("yyyyMMdd") + '-' + "001";
            }
            decimal ss = 0;
            var name = "tpnzvede@hotmail.com";
            foreach (Product pr in cart)
            {
                Product pp = _context.Products.Where(a => a.ProductCode == pr.ProductCode).FirstOrDefault();

                //c.OrderId = code;
                Cart ca = new Cart()
                {
                    OrderId = code,
                    Customer = "another@test.com",
                    Price = pp.Price,
                    Quantity = 1,
                    CreatedOn = DateTime.Now,
                    ProductName = pp.ProductName
                };
                _context.Carts.Add(ca);
                _context.SaveChanges();
                pp.Quantity -= 1;
                _context.Products.Update(pp);
                _context.SaveChanges();
                ss = ss + pr.Price;
            }
            Order o = new Order()
            {
                OrderId = code,
                Customer = "another@test.com",
                Processed = false,
                GrandTotal = ss
            };
            _context.Orders.Add(o);
            await _context.SaveChangesAsync();
            Order od = _context.Orders.Where(a => a.OrderId == code).FirstOrDefault();
            od.Processed = true;
            _context.Orders.Update(od);
            _context.SaveChanges();

            var message = new Message(new string[] { name },
              $"Thanks for the order {code}", $"Thank you for the purchase of r{ss}. " +
              $"It was good doing business with you <br> <br> Please call agan!! <br> Tested Online Store.", null);
            await _emailSender.SendEmailAsync(message);
            //send email and update table
            return CreatedAtAction("GetCart", new { id = code }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
