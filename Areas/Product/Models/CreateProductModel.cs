using System.ComponentModel;

namespace app.areas.models  {
    public class CreateProductModel : ProductModel {
            [DisplayName("chuyên mục")]
            public int[] CategoryIds {get;set;}
    }
}