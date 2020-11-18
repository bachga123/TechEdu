using BaiTapTuan7.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BaiTapTuan7.Controllers
{
    public class HomeController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangNhap()
        {
            ViewBag.Message = "Login";
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string taikhoan = f["username"].ToString();
            string matkhau = f["password"].ToString();
            //string matkhaumd5 = GetMD5(matkhau);
            tb_Users us = db.tb_Users.SingleOrDefault(n => n.Username == taikhoan && n.Password == matkhau);
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                Session["user"] = us;
                Session["userName"] = us.Username;

                if (us.Usertype == "admin")
                {
                    return Content("/Admin");
                }
                else if (us.Usertype == "student")
                {
                    return Content("/Student");
                }
                else if (us.Usertype == "teacher")
                {
                    return Content("/Teacher");
                }
            }
            return Content("false");
        }
        
    }
}