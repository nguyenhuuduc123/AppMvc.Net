using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.models.Product.photo {

    [Table("ProductPhoto")]
    public class ProductPhoto{
        [Key]
        public int Id {set;get;}
        public int ProductId {get;set;}
        public string FileName {set;get;}
        [ForeignKey("ProductId")]
        public ProductModel product {get;set;}
    }
}