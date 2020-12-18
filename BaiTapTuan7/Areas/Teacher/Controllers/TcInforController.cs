using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
namespace BaiTapTuan7.Areas.Teacher.Controllers
{
    [AuthorizeController]
    public class TcInforController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Teacher/Student
        public ActionResult Index()
        {
            ViewBag.newsList = MyNews();
            return View();
        }
        public ActionResult TeacherProfile()
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            var tcc = db.tb_Teacher.Find(tc.TeacherId);
            return View("TeacherProfile", tcc);
        }
        [HttpPost]
        public ActionResult TeacherProfile(tb_Teacher tc)
        {
            if (ModelState.IsValid)
            {
                tb_Teacher oldtc = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == tc.TeacherId);
                if (oldtc != null)
                {
                    oldtc.TeacherFirstName = tc.TeacherFirstName;
                    oldtc.TeacherLastName = tc.TeacherLastName;
                    oldtc.DateOfBirth = tc.DateOfBirth;
                    oldtc.Gmail = tc.Gmail;
                    oldtc.PhoneNumber = tc.PhoneNumber;
                    oldtc.PlaceOfBirth = tc.PlaceOfBirth;
                    HttpPostedFileBase upload = Request.Files["image"];
                    if (upload.FileName != "")
                    {
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            oldtc.Images = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    db.Entry(oldtc).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("TeacherProfile", oldtc);
                }
                else
                {
                    ModelState.AddModelError("", "Edit teacher failed 1");
                    return View("TeacherProfile", tc);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit Student failed 2 ");
                return View("TeacherProfile", tc);
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
        public ActionResult NewsDetails(int newsid)
        {
            var news = db.tb_News.Find(newsid);
            return View("NewsDetails", news);
        }
        public List<tb_News> MyNews()
        {
            var newsLists = db.tb_News.Where(m => m.To == "3" || m.To == "1").ToList();
            return newsLists;
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