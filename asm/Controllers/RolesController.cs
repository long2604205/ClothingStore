using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> manager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            manager = roleManager;
        }


        public IActionResult Index()
        {
            string[] roleNames = 
            { 
                "Admin", 
                "Customer", 
                "Manager",
                "Staff",
            };

            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                var roleExist = manager.RoleExistsAsync(roleName).GetAwaiter().GetResult();

                if (!roleExist)
                {
                    // Create the role
                    manager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
            }

            var roles = manager.Roles;
            return View(roles);
        }

        // public IActionResult Index()
        // {
        //     var roles = manager.Roles;
        //     return View(roles);
        // }

        // [HttpGet]
        // public IActionResult Create()
        // {
        //     return View();
        // }

        // [HttpPost]
        // public IActionResult Create(IdentityRole role)
        // {
        //     //check if the roles exist
        //     if (!manager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
        //     {
        //         manager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
        //     }
        //     return RedirectToAction("Index");
        // }

        // [HttpPost]
        // public IActionResult Create()
        // {
        //     string[] roleNames = { "role1", "role2", "role3" };

        //     foreach (var roleName in roleNames)
        //     {
        //         // Check if the role already exists
        //         var roleExist = manager.RoleExistsAsync(roleName).GetAwaiter().GetResult();

        //         if (!roleExist)
        //         {
        //             // Create the role
        //             manager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
        //         }
        //     }

        //     return RedirectToAction("Index");
        // }

    }
}

