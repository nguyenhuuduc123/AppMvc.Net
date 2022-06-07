using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using App.mvc.net.Contacts;
using razorweb.models;

namespace App.mvc.net.Models {
        public class AppDbContext : IdentityDbContext<AppUser> {

        public AppDbContext (DbContextOptions<AppDbContext> options) : base (options) { }

        protected override void OnModelCreating (ModelBuilder builder) {

            base.OnModelCreating (builder);
            // Bỏ tiền tố AspNet của các bảng: mặc định
            foreach (var entityType in builder.Model.GetEntityTypes ()) {
              var tableName = entityType.GetTableName ();
               if (tableName.StartsWith ("AspNet")) {
                     entityType.SetTableName (tableName.Substring (6));
                }
           }
         }
        public DbSet<Contact> Contacts {set;get;}
    }

}