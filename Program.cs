using Microsoft.AspNetCore.Mvc.Razor;
using App.mvc.net.Controllers;
using Microsoft.AspNetCore.Routing.Constraints;
using App.mvc.Services;

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
builder.Services.AddSingleton<PlanetServices>();
builder.Services.AddSingleton(typeof(ProductServices),typeof(ProductServices));
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