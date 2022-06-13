namespace app.models{
    // app.models.Summernote
    public class Summernote {
        public Summernote(string iedittor,bool loadlibrary = true){
            IEditor = iedittor;
            LoadLibrary = loadlibrary;
        }
        public string IEditor {get;set;}
        public bool LoadLibrary {get;set;}
        public  int height {set;get;} = 120;
        public string toolbar {set;get;} = @"[
          ['style', ['style']],
         ['font', ['bold', 'underline', 'clear']],
         ['color', ['color']],
         ['para', ['ul', 'ol', 'paragraph']],
         ['table', ['table']],
         ['insert', ['link', 'picture', 'video','elfinder']],
          ['height',['height']],
          ['view', ['fullscreen', 'codeview', 'help']]
       ]";
    }

}
// <script>
//      $(document).ready(function(){
//          $('#summernoteEditor').summernote({
//         tabsize: 2,
//         height: 120,
//         toolbar: [
//           ['style', ['style']],
//           ['font', ['bold', 'underline', 'clear']],
//           ['color', ['color']],
//           ['para', ['ul', 'ol', 'paragraph']],
//           ['table', ['table']],
//           ['insert', ['link', 'picture', 'video']],
//           ['height',['height']],
//           ['view', ['fullscreen', 'codeview', 'help']]
//         ]
//       });
//      })
      
//     </script>