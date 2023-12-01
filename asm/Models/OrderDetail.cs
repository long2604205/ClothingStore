using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace asm.Models
{
	public class OrderDetail
	{
		public int ID { get; set; }
		public int Qty { get; set; }
		public double Price { get; set; }
		public string? Size { get; set; }
		public DateTime? Created_at { get; set; }
		public DateTime? Updated_at { get; set; }

		[ForeignKey("ProductID")]
		public int ProductID { get; set; }
		public Product? Product { get; set; }

		[ForeignKey("OrderID")]
		public int OrderID { get; set; }
		public Order? Order { get; set; }
	}
}

