using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Customer { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? Price { get; set; }

        public string OrderId { get; set; }
    }
}
