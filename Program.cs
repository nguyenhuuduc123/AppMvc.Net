using Microsoft.AspNetCore.Mvc.Razor;
using App.mvc.net.Controllers;

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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints => {
    // say hi
    endpoints.MapGet("/sayhi",async context => {
            await context.Response.WriteAsync($"heelo asp mvc {DateTime.Now}");
    });
    // endpoints.MapControllers
    // endpoints.MapControllerRoute
     //endpoints.MapDefaultControllerRoute
     //endpoints.MapAreaControllerRoute
     endpoints.MapControllerRoute(
         name : "first_route",
         pattern : "start_here", // /start_here
         defaults : new {
             controller = "First",
             action = "Iphoneprice",
             id = 3
         } 
     );
    endpoints.MapRazorPages();
});

app.Run();
