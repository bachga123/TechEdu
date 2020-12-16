using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BaiTapTuan7.Models;
namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class AdminController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        // Thông tin cá nhân của admin (admin ví dụ là một teacher)
        public ActionResult Logout()
        {
            ClearCache();
            return RedirectToAction("Index", "Home",new { @area = ""});
        }
        public ActionResult AdminProfile()
        {
            tb_Teacher tc = (tb_Teacher)Session["admin"];
            var tcc = db.tb_Teacher.Find(tc.TeacherId);
            return View("AdminProfile", tcc);
        }
        [HttpPost]
        public ActionResult AdminProfile(tb_Teacher tc)
        {
            if (ModelState.IsValid)
            {
                tb_Teacher oldtc = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == tc.TeacherId);
                if (oldtc != null)
                {
                    oldtc.TeacherFirstName = tc.TeacherFirstName;
                    oldtc.TeacherLastName = tc.TeacherLastName;
                    oldtc.DateOfBirth = tc.DateOfBirth;
                    oldtc.PhoneNumber = tc.PhoneNumber;
                    oldtc.Gmail = tc.Gmail;
                    oldtc.PlaceOfBirth = tc.PlaceOfBirth;
                    HttpPostedFileBase upload = Request.Files["image"];
                    if (upload.FileName != "")
                    {
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            oldtc.Images = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    db.Entry(oldtc).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("AdminProfile",oldtc);
                }
                else
                {
                    ModelState.AddModelError("", "Edit teacher failed 1");
                    return View("AdminProfile", tc);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit Student failed 2 ");
                return View("AdminProfile", tc);
            }
        }
        public ActionResult AccountSetting()
        {
            ViewBag.teacher = (tb_Teacher)Session["teacher"];
            tb_Users us = (tb_Users)Session["user"];
            var uss = db.tb_Users.Find(us.Id);
            return View("AccountSetting", uss);
        }
        [HttpPost]
        public ActionResult AccountSetting(tb_Users us)
        {
            if (ModelState.IsValid)
            {
                var uss = db.tb_Users.Find(us.Id);
                if (uss != null)
                {
                    uss.Password = us.Password;
                    db.Entry(uss).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AccountSetting");
                }
                else
                {
                    ModelState.AddModelError("", "Edit User failed");
                    return RedirectToAction("AccountSetting", us);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit User failed");
                return RedirectToAction("AccountSetting", us);
            }

        }
        public void ClearCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
        
    }
}