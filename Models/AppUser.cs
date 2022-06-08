using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace razorweb.models{
    // using razorweb.models
    public class AppUser :IdentityUser{
        [Column(TypeName = "nvarchar")]
        [StringLength(255)]
            public string EmailAddress{get;set;}
            [DataType(DataType.Date)]
            public DateTime? BirthDay {set;get;}
    }
}