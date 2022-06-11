using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.mvc.net.Models;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcblog.Models;
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
                var useradmin = await _userManager.FindByEmailAsync("admin@example.com");
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
                seedPostCategory();
                statusmessage = "vừa seed database";
                return RedirectToAction("Index");
            
        }
            private void seedPostCategory(){
        _dbContext.Categories.RemoveRange(_dbContext.Categories.Where(c => c.Content.Contains("[fakeDate]")));
         _dbContext.posts.RemoveRange(_dbContext.posts.Where(p => p.Content.Contains("[fakeDate]")));
                var fakerCategoy = new Faker<Category>();
                int cm = 1;
                fakerCategoy.RuleFor(c => c.Title, fk => $"CM{cm++}" + fk.Lorem.Sentence(1,2).Trim('.'));
                fakerCategoy.RuleFor(c => c.Content, fk => fk.Lorem.Sentence(5) + "[fakeData]");
                fakerCategoy.RuleFor(c => c.Slug,fk => fk.Lorem.Slug());



                var cate1 = fakerCategoy.Generate();
                var cate11 = fakerCategoy.Generate();
                var cate12 =  fakerCategoy.Generate();
                var cate2 = fakerCategoy.Generate();
                var cate21 = fakerCategoy.Generate();
                var cate211 = fakerCategoy.Generate();


                cate11.ParentCategory = cate1;
                cate12.ParentCategory = cate1;
                cate21.ParentCategory = cate2;
                cate211.ParentCategory = cate21;
                var categories = new Category[] {
                    cate1,cate11,cate12,cate2,cate21,cate211
                };
                _dbContext.Categories.AddRange(categories);

                // POST
                var rCategory = new Random();
                int bv = 1;
                var user = _userManager.GetUserAsync(this.User).Result;
                var fakerPost = new Faker<Post>();
                fakerPost.RuleFor(p => p.AuthorId,f => user.Id);
                fakerPost.RuleFor(p => p.Content,f => f.Lorem.Paragraphs(7)+"[fakeData]");
                fakerPost.RuleFor(p => p.DateCreated,f => f.Date.Between(new DateTime(2022,1,1),new DateTime(2022,7,26)));
                fakerPost.RuleFor(p => p.Description,f => f.Lorem.Sentences(3));
                fakerPost.RuleFor(p => p.Published,f => true);
                fakerPost.RuleFor(p => p.Slug,f => f.Lorem.Slug());
                fakerPost.RuleFor(p => p.Title,f => $"bai {bv++}"+ f.Lorem.Sentence(3,4).Trim(','));

                List<Post> posts = new List<Post>();
                List<PostCategory> post_caregories = new List<PostCategory>();


                for(int i = 0; i< 40; i++){
                    var post = fakerPost.Generate();
                    post.DateUpdated = post.DateCreated;
                    posts.Add(post);
                    post_caregories.Add(new PostCategory(){
                        Post = post,
                        Category = categories[rCategory.Next(5)]
                    });
                }
                _dbContext.AddRange(posts);



                _dbContext.SaveChanges();

        }

    }
}