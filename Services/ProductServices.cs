namespace App.mvc.net.Controllers{
    public class ProductServices : List<ProductModel>
    {
        public ProductServices(){
            this.AddRange(new List<ProductModel>{
                new ProductModel{id = 1,Name = "IphoneX", Price = 5000},
                new ProductModel{id = 2,Name = "nokia", Price = 6000},
                new ProductModel{id = 3,Name = "samsum", Price = 7000},
                new ProductModel{id = 4,Name = "cucgach", Price = 8000},
            });
        }
    }
}