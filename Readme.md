## Controller
- là một lớp kế thừa từ thư viện  : Microsoft.AspNetCore.Mvc.Controller
- Action trong cotroller là một phương thức public (không được static)
- Action trả về bất kỳ kiểu dữ liệu nào : thường là IActionResult 
- các dịch vụ inject vào controller qua hàm tạo
## View
- là file cshtml
- View cho Action lưu tại : /View/ControlllerName/ActionName.cshtml
-// myview/controller/action.cshtml
         // {0} =>    tên action
         // {1} => tên controller
         // {2} => tên areas
         options.ViewLocationFormats.Add("/myview/{1}/{0}" + RazorViewEngine.ViewExtension);
## truyền dữ liệu sang View
- Model 
-ViewData
-ViewBag
-TempData
# Areas 
- là tên dùng để routing
- là cấu trúc thư mục chứa M.V.C
- thiết lập Area cho controller bằng ....[Area("AreaName")]
- tạo cấu trúc thư mục 
dotnet aspnet-codegenerator area productmanage