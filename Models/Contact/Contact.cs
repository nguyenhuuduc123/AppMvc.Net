using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.mvc.net.Contacts{
        public class Contact {
            [Key]
            public int Id {set;get;}
            [Column(TypeName = "nvarchar")]
            [StringLength(50)]
            [Required(ErrorMessage ="phải nhập {0}")]
            [Display(Name = "họ và tên")]
            public string FulName {set;get;}
             [Required(ErrorMessage ="phải nhập {0}")]
            [StringLength(100)]
            [EmailAddress(ErrorMessage = "phải là địa chỉ email")]
            public string Email {set;get;}
            public DateTime DateSent {set;get;}
             [Display(Name ="nội dung")]
            public string Message {set;get;}
            [StringLength(50)]
            [Phone(ErrorMessage ="phải là số điện thoại")]
            [Display(Name =" số điện thoại")]
            public string Phone {set;get;}
        }
}