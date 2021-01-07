using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
using BaiTapTuan7.Areas.Teacher.Model;
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
            ViewBag.courseAssignmentList = CourseAssignmentActiveList();
            ViewBag.myCourseList = MyCourseList();
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
                    var us = db.tb_Users.Find(oldtc.UserId);
                    us.Email = tc.Gmail;
                    oldtc.PhoneNumber = tc.PhoneNumber;
                    oldtc.PlaceOfBirth = tc.PlaceOfBirth;
                    HttpPostedFileBase upload = Request.Files["image"];
                    if (upload.FileName != "")
                    {
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            oldtc.Images = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    db.Entry(us).State = EntityState.Modified;
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
                if (us.Password != null)
                {
                    var uss = db.tb_Users.Find(us.Id);
                    if (uss != null)
                    {
                        uss.Password = MD5(us.Password);
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
                    return View("AccountSetting", us);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit User failed");
                return View("AccountSetting", us);
            }

        }
        public ActionResult NewsDetails(int newsid)
        {
            ViewBag.newsList = MyNews();
            var news = db.tb_News.Find(newsid);
            return View("NewsDetails", news);
        }

        public List<CourseAssignment> CourseAssignmentActiveList()
        {
            List<CourseAssignment> caList = new List<CourseAssignment>();
            var couList = MyCourseList();
            foreach(var item in couList)
            {
                CourseAssignment ca = new CourseAssignment();
                ca.CourseId = item.Course_Id;
                ca.CourseName = item.Course_Name;
                ca.AssignmentList = AssignmentList(item.Course_Id);
                caList.Add(ca);
            }
            return caList;
        }
        public List<tb_Assignment> AssignmentList(int couid)
        {
            var assList = db.tb_Assignment.Where(m => m.Course_Id == couid && m.Status == 1).ToList();
            return assList;
        }

        public List<tb_Course> MyCourseList()
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            var data = db.tb_Course.Where(m => m.TeacherId == tc.TeacherId);
            List<tb_Course> myCourseList = new List<tb_Course>();
            foreach (var item in data)
            {
                var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.Course_Id && m.TeacherId == tc.TeacherId);
                if (myCourseList.Contains(cou) == false)
                    myCourseList.Add(cou);
            }
            return myCourseList;
        }

        public List<tb_News> MyNews()
        {
            var newsLists = db.tb_News.Where(m => m.To != "2").ToList();
            return newsLists;
        }
        public ActionResult Logout()
        {
            ClearCache();
            return RedirectToAction("Index", "Home", new { @area = "" });
        }
        private static string MD5(string Metin)
        {
            MD5CryptoServiceProvider MD5Code = new MD5CryptoServiceProvider();
            byte[] byteDizisi = Encoding.UTF8.GetBytes(Metin);
            byteDizisi = MD5Code.ComputeHash(byteDizisi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in byteDizisi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
        public void ClearCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}