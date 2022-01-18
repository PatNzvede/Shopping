using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingAPI.Models
{
    public class Product
    {
        public int id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public decimal Price { get; set; }
        public string Picture { get; set; }
        public int Quantity { get; set; }
        public int? CreatedBy { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
