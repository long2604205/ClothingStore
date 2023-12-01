using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace asm.Models
{
    public class ProductSize
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int SizeID { get; set; }

        [Required(ErrorMessage = "StockQuantity is required.")]
        public int StockQuantity { get; set; }

        public Product? Product { get; set; }
        public Size? Size { get; set; }

        // Mối quan hệ ngược lại đến CartItem
        // public ICollection<CartItem>? CartItems { get; set; }
    }
}

