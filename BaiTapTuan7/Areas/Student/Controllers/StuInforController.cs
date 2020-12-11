using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;

namespace BaiTapTuan7.Areas.Student.Controllers
{
    [AuthorizeController]
    public class StuInforController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Student/Student
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StudentProfile()
        {
            //tb_Users us = (tb_Users)Session["user"];
            //ViewBag.us = us;
            //tb_Student stu = db.tb_Student.FirstOrDefault(m => m.UserId == us.Id);
            tb_Student stu = (tb_Student)Session["student"];
            var stuu = db.tb_Student.Find(stu.StudentId);
            return View("StudentProfile", stuu);
        }
        [HttpPost]
        public ActionResult StudentProfile(tb_Student stu)
        {
            if (ModelState.IsValid)
            {
                tb_Student oldstu = db.tb_Student.FirstOrDefault(m => m.StudentId == stu.StudentId);
                if (oldstu != null)
                {
                    oldstu.FirstName = stu.FirstName;
                    oldstu.LastName = stu.LastName;
                    oldstu.Gmail = stu.Gmail;
                    oldstu.PhoneNumber = stu.PhoneNumber;
                    oldstu.DateOfBirth = stu.DateOfBirth;
                    oldstu.PlaceOfBirth = stu.PlaceOfBirth;
                    HttpPostedFileBase upload = Request.Files["image"];
                    if (upload.FileName != "")
                    {
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            oldstu.Images = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    db.Entry(oldstu).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("StudentProfile", oldstu);
                }
                else
                {
                    ModelState.AddModelError("", "Edit Student failed 1");
                    return View("StudentProfile", stu);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit Student failed 2 ");
                return View("StudentProfile", stu);
            }
        }
        public ActionResult AccountSetting()
        {
            ViewBag.student = (tb_Student)Session["student"];
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
                if(uss != null)
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