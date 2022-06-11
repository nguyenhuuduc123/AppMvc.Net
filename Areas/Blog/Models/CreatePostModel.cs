using System.ComponentModel;

namespace app.areas.models  {
    public class CreatePostModel : Post {
            [DisplayName("chuyên mục")]
            public int[] CategoryIds {get;set;}
    }
}