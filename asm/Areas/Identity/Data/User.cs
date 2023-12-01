using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using asm.Models;
using Microsoft.AspNetCore.Identity;

namespace asm.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Postcode { get; set; }
    public int? Status { get; set; }
    public int? Gender { get; set; }
    public string? Image { get; set; }
    [NotMapped]
    public IFormFile? File { get; set; }
    public ICollection<Order>? Orders { get; set; }

}

