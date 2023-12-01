using System;
using System.ComponentModel.DataAnnotations.Schema;
using asm.Areas.Identity.Data;
namespace asm.Models
{
	public class Order
	{
		public int ID {get; set;}
		public string? Code {get; set;}
		public double TotalAmount {get; set;}
		public string? Payment {get; set;}
		public string? Note {get; set;}
		public int Status {get; set;}
		public DateTime? Created_at {get; set;}
		public DateTime? Updated_at {get; set;}

		public string? Email {get; set;}

        [ForeignKey("UserID")]
        public string? UserID { get; set; }
		public User? User {get; set;}
		public ICollection<OrderDetail>? OrderDetails  { get; set; }
	}
}

