using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
namespace BaiTapTuan7.Areas.Teacher.Controllers
{
    [AuthorizeController]
    public class TcInforController : Controller
    {
        // GET: Teacher/Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TeacherProfile()
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            return View("TeacherDetails", tc);
        }

        public ActionResult Logout()
        {
            ClearCache();
            return RedirectToAction("Index", "Home", new { @area = "" });
        }
        public void ClearCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}