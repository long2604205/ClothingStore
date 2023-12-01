using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace asm.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(255, ErrorMessage = "Product Name must not exceed 255 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive whole number.")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Summary is required.")]
        public string? Summary { get; set; }

        [Required(ErrorMessage = "Sku is required.")]
        public string? Sku { get; set; }

        [Required(ErrorMessage = "Alias is required.")]
        public string? Alias { get; set; }

        //[Required(ErrorMessage = "Image is required.")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Images is required.")]
        public List<string>? Images { get; set; }
        //public string? Images { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 5, ErrorMessage = "Status must be 0 to 5.")]
        public int? Status { get; set; }

        public int? CategoryID { get; set; }

        public DateTime? Created_at { get; set; }

        public DateTime? Updated_at { get; set; }

        public Category? Category { get; set; }

        public ICollection<ProductSize>? Sizes { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }


        [NotMapped]
        public IFormFile? File { get; set; }

        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }

        public Product()
        {
            // Khởi tạo danh sách nếu cần thiết
            Images = new List<string>();
        }
    }
}

