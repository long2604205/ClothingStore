using System;
using System.ComponentModel.DataAnnotations;

namespace asm.Models
{
	public class Category
	{
        public int ID { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(255, ErrorMessage = "Product Name must not exceed 255 characters.")]
        public string? Name { get; set; }

        [Range(0, 5, ErrorMessage = "Status must be 0 or 1.")]
        public int Status { get; set; }

        public DateTime Created_at { get; set; }

        public DateTime Updated_at { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

