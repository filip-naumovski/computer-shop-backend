using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerShopBackend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string PhotoUrl { get; set; }
        [Display(Name = "Cart")]
        public int? CartId { get; set; }
        public Cart Cart { get; set; }
        [Display(Name = "Order")]
        public int? OrderId{ get; set; }
        public Order Order { get; set; }
    }
}
