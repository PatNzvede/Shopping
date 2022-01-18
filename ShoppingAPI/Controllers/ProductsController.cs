using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingAPI.Models;

namespace ShoppingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public ProductsController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] Product product)
        {
            if (product.Image.Length > 0)
            {
                string wwwRootPath = hostEnvironment.WebRootPath;
                string newPath = Path.GetFullPath(Path.Combine(wwwRootPath, @"..\..\"));
                string angular = Path.Combine(System.IO.Directory.GetCurrent‌​Directory());
                string fileName = Path.GetFileNameWithoutExtension(product.Image.FileName);
                string extension = Path.GetExtension(product.Image.FileName);
                product.Picture = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images/", product.Picture);
                string path1 = Path.Combine(newPath + "/ShoppingUI/src/assets/", product.Picture);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.Image.CopyToAsync(fileStream);
                }
                using (var fileStream1 = new FileStream(path1, FileMode.Create))
                {
                    await product.Image.CopyToAsync(fileStream1);
                }
                var code = "";
                Product pr = _context.Products.OrderByDescending(a => a.id).FirstOrDefault();
                if (pr != null && product.ProductCode != "001")
                {
                    string[] th = pr.ProductCode.Split('-');

                    if (th[0] == DateTime.Today.ToString("yyyyMM"))
                    {
                        int idd = Int32.Parse(th[1]);
                        idd++;
                        code = th[0] + '-' + idd.ToString("D3");
                    }
                    else
                    {
                        code = DateTime.Today.ToString("yyyyMM") + '-' + "001";
                    }
                }
                else
                {
                    code = DateTime.Today.ToString("yyyyMM") + '-' + "001";
                }
                product.ProductCode = code;
                Product pd = new Product()
                {
                    ProductCode = product.ProductCode,
                    // Category = product.Category,
                    Picture = product.Picture,
                    ProductName = product.ProductName,
                    CreatedOn = DateTime.Now,
                    CreatedBy = 1,
                    Quantity = product.Quantity,
                    Price = product.Price
                };
                _context.Products.Add(pd);

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.id }, product);
            }
            else
            {
                return NotFound();
            }
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.id == id);
        }
    }
}
