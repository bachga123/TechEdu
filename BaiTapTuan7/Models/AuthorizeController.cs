using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BaiTapTuan7.Models
{
    public class AuthorizeController : ActionFilterAttribute
    {
        TechEduEntities db = new TechEduEntities();
        //phương thức thực thi khi action được gọi
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            tb_Users tbus = HttpContext.Current.Session["user"] as tb_Users;
            //nếu session=null thì trả về trang đăng nhập
            if (tbus == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
            //session != null
            else
            {
                //lấy danh sách quyền của user
                var us_quyen = db.tb_UserType.Where(n => n.UserTypeId == tbus.Usertype);
                //đếm số lượng quyền
                int slquyen = us_quyen.Count();
                //khởi tạo mảng
                string[] listpermission = new string[slquyen];
                int i = 0;
                //lấy danh sách quyền đưa vào mảng
                foreach (var item in us_quyen)
                {
                    listpermission[i] = item.UserTypeName;
                    i++;
                }
                //Lấy tên controller và action
                //string actionName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "-" + filterContext.ActionDescriptor.ActionName;

                //Lấy tên Controller
                string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                //nếu tên controler không có trong mảng quyền của user thì trả về trang đăng nhập
                if (!listpermission.Contains(ControllerName))
                {
                    filterContext.Result = new RedirectResult("~/Home/Index");
                }
            }
        }
    }
    
}