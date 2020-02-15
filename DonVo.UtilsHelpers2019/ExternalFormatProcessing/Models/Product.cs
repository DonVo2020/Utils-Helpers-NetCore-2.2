using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalFormatProcessing.Models
{
    public class Product
    {
        public Product()
        {
            this.ProductCategories = new HashSet<CategoryProduct>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int? BuyerId { get; set; }
        public User Buyer { get; set; }

        public int SellerId { get; set; }
        public User Seller { get; set; }

        public ICollection<CategoryProduct> ProductCategories { get; set; }
    }
}
