using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using asm.Data;
using asm.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Http.Features;
using asm.Models;
using Microsoft.CodeAnalysis.Options;
using System.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
});




// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<asmIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<asmIdentityDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1440); // Thời gian chờ trước khi Session hết hiệu lực
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.Configure<PaypalSettings>(builder.Configuration.GetSection("PaypalSettings"));

var clientId = builder.Configuration["PaypalSettings:ClientId"];
var secret = builder.Configuration["PaypalSettings:Secret"];
var url = builder.Configuration["PaypalSettings:Url"];


//builder.Services.AddSingleton<ICartService, CartService>();

//builder.Services.AddScoped<ICartService, CartService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
// Sử dụng Session Middleware
app.UseSession();

//app.MapControllerRoute(
//    name: "Admin",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
name: "404",
pattern: "/{**catchall}",
defaults: new { controller = "Home", action = "Error" }
);
app.MapControllerRoute(
    name: "Administrator",
    pattern: "/administrator/{controller=Home}/{action=Index}/{id?}");


//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//    endpoints.MapControllerRoute(
//        name: "Admin",
//        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
//    );

//    endpoints.MapControllerRoute(
//        name: "CatchAll",
//        pattern: "{*url}",
//        defaults: new { controller = "Home", action = "Error" }
//    );

//    //endpoints.MapControllerRoute(
//    //name: "404",
//    //pattern: "/{**catchall}",
//    //defaults: new { controller = "Home", action = "Error" }
//    //);
//});

app.MapRazorPages();

app.Run();

