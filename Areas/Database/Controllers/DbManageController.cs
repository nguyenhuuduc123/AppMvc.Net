using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.mvc.net.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using razorweb.models;

namespace App.mvc.net.Areas_Database_Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbManageController(AppDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [TempData]
        public string statusmessage { get; set; }
        
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _dbContext.Database.EnsureDeletedAsync();
            statusmessage = success ? "xóa thành công" : "không xóa được";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _dbContext.Database.MigrateAsync();
            statusmessage = "cập nhập database thành công";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> SeedDataAsync()
        {


            var roleNames = typeof(RoleName).GetFields().ToList();
            foreach (var r in roleNames)
            {
                var rolename = (string)r.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }
                // adimn : pass = admin123 admin@example.com
                var useradmin = await _userManager.FindByEmailAsync("adimn");
                if (useradmin == null)
                {
                    useradmin = new AppUser()
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                    };
                    await _userManager.CreateAsync(useradmin, "admin123");
                    await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
                }
                statusmessage = "vừa seed database";
                return RedirectToAction("Index");
            
        }

    }
}