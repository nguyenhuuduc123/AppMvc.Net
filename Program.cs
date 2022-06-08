using Microsoft.AspNetCore.Mvc.Razor;
using App.mvc.net.Controllers;
using Microsoft.AspNetCore.Routing.Constraints;
using App.mvc.Services;
using App.mvc.net.Models;
using Microsoft.EntityFrameworkCore;
using razorweb.models;
using Microsoft.AspNetCore.Identity;
using App.Services;
using App.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<RazorViewEngineOptions>(options => {
        // myview/controller/action.cshtml
         // {0} =>    tên action
         // {1} => tên controller
         // {2} => tên areas
         options.ViewLocationFormats.Add("/myview/{1}/{0}" + RazorViewEngine.ViewExtension);
});
builder.Services.AddOptions();
var mailsetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsetting);
builder.Services.AddSingleton< IEmailSender,SendMailService>();
builder.Services.AddDbContext<AppDbContext>(options =>{
        string connection = builder.Configuration.GetConnectionString("AppMvcConnectionString");
        options.UseSqlServer(connection);
}
);
builder.Services.AddIdentity<AppUser, IdentityRole>().
AddEntityFrameworkStores<AppDbContext>().
AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt
    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất
    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = false; // xác thực email trước khi đăng nhập
});
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});
 //builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
builder.Services.AddAuthentication().
 AddGoogle(options => {
   var  ggcofig =  builder.Configuration.GetSection("Authentication:Google");
     options.ClientId = ggcofig["ClientId"];
     options.ClientSecret = ggcofig["ClientSecret"];
     // địa chỉ mặc định của CallbackPath là signin-google
     options.CallbackPath = "/dang-nhap-tu-google";
 });
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
builder.Services.AddSingleton<PlanetServices>();
builder.Services.AddSingleton(typeof(ProductServices),typeof(ProductServices));
builder.Services.AddAuthorization(options => {
    options.AddPolicy("ViewManageMenu",builder =>{
        builder.RequireAuthenticatedUser();
        builder.RequireRole(RoleName.Administrator);
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePages();

app.UseRouting();
app.UseAuthentication(); // xác định danh tính
app.UseAuthorization(); // xacs thực quyền truy cập
app.MapAreaControllerRoute(
      name : "first_route",
         pattern : "{controller}/{action=Index}/{id?}",
         areaName : "productmanage"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    // say hi
  //  endpoints.MapGet("/sayhi",async context => {
  //          await context.Response.WriteAsync($"hello asp mvc {DateTime.Now}");
  //  });
    // endpoints.MapControllers
    // endpoints.MapControllerRoute
     //endpoints.MapDefaultControllerRoute
     //endpoints.MapAreaControllerRoute
     // defaults : new {
    //    controller =>
    //      action => 
    //   area =>
    //  atribute
     //}
     endpoints.MapControllerRoute(
         name : "first",
         pattern : "{url}/{id:range(2,4)}",
         defaults : new {
            controller = "First",
            action = "ViewProduct",        
         },
        constraints : new {
            url =  new RegexRouteConstraint("^((xemsanpham)|(viewproduct))$"),
          // id = new RangeRouteConstraint(2,4) 
           }                        //new StringRouteConstraint("xemsanpham"),

         
         
     );
    

     
     endpoints.MapControllerRoute(
         name : "first_route",
         pattern : "{controller=Home}/{action=Index}/{id?}"// /start_here
        //  defaults : new {
        //      controller = "First",
        //      action = "ViewProduct",
        //      id = 3
        //  } 
     );
    endpoints.MapRazorPages();
});

app.Run();

// dotnet aspnet-codegenerator controller -name Planet -namespace App.mvc.net.Controllers -outDir Controllers
// dotnet aspnet-codegenerator controller -name Product -namespace App.mvc.net.Controllers -outDir Controllers
//dotnet aspnet-codegenerator area Database
//dotnet aspnet-codegenerator controller -name DbManage -outDir Areas/Database/Controllers
//dotnet aspnet-codegenerator controller -name Contact -namespace App.Areas.Contact.Controllers -m App.mvc.net.Contacts.Contact -udl -dc App.mvc.net.Models.AppDbContext -outDir Areas/Contact/Controllers/