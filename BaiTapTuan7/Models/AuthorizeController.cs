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
                ////lấy danh sách quyền của user
                //var us_quyen = db.tb_UserType.Where(m => m.UserTypeId == tbus.Usertype);
                
                ////đếm số lượng quyền
                //int slquyen = us_quyen.Count();
                ////khởi tạo mảng
                //string[] listpermission = new string[slquyen];
                //int i = 0;
                ////lấy danh sách quyền đưa vào mảng
                //foreach (var item in us_quyen)
                //{
                //    listpermission[i] = item.UserTypeName;
                //    i++;
                //}
                ////Lấy tên controller và action
                ////string actionName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "-" + filterContext.ActionDescriptor.ActionName;
                ////Lấy tên Controller
                //string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                ////nếu tên controler không có trong mảng quyền của user thì trả về trang đăng nhập
                //if (!listpermission.Contains(ControllerName))
                //{
                //    filterContext.Result = new RedirectResult("~/Home/Index");
                //}

                var us_type_role = db.tb_UserType_Roles.Where(m => m.UserTypeId == tbus.Usertype);
                int slquyen = us_type_role.Count();
                //khởi tạo mảng
                string[] listpermission = new string[slquyen];
                int i = 0;
                foreach (var item in us_type_role)
                {
                    var us_role = db.tb_Roles.Where(m => m.RoleId == item.RoleId);
                    foreach(var item1 in us_role)
                    {
                        listpermission[i] = item1.RoleName;
                        i++;
                    }
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