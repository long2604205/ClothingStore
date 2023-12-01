using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace asm.Models
{
	public class Size
	{
        public int? ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        public ICollection<ProductSize>? Products { get; set; }
    }
}

