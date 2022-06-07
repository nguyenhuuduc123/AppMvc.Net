using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.mvc.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.mvc.net.Areas_Database_Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _dbContext;
        public DbManageController(AppDbContext dbContext){
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb(){
            return View();
        }
        [TempData]
        public string statusmessage {get;set;}
       [HttpPost]
        public async Task<IActionResult> DeleteDbAsync(){
             var success = await _dbContext.Database.EnsureDeletedAsync();
             statusmessage = success ? "xóa thành công" : "không xóa được";
             return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Migrate(){
              await _dbContext.Database.MigrateAsync();
             statusmessage = "cập nhập database thành công";
             return RedirectToAction(nameof(Index));
        }
    }
}