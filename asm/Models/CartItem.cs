using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace asm.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string? Image {get; set;}
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? SizeName { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        // Ánh xạ mối quan hệ rõ ràng
        [ForeignKey("CartId")]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
    }
}