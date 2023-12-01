using System;

namespace asm.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        // public List<CartItem> CartItems { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}