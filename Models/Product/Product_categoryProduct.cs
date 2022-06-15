using System.ComponentModel.DataAnnotations.Schema;
using mvcblog.Models;

[Table("Product_categoryProduct")]
public class Product_categoryProduct
{
    public int ProductId {set; get;}

    public int CategoryID {set; get;}

    [ForeignKey("ProductId")]
    public ProductModel Product {set; get;}

    [ForeignKey("CategoryID")]
    public CategoryProduct Category {set; get;}
}