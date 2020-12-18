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
            ViewBag.myDeadlineList = MyDeadlineList();
            ViewBag.newsList = MyNews();
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
        // News
        public ActionResult NewsDetails(int newsid)
        {
            var news = db.tb_News.Find(newsid);
            return View("NewsDetails", news);
        }
        public ActionResult Logout()
        {
            ClearCache();
            return RedirectToAction("Index", "Home", new { @area = "" });
        }
        public List<tb_Course> MyCourseList()
        {
            tb_Student stu = (tb_Student)Session["student"];
            var data = db.tb_StudentCourse.Where(m => m.StudentId == stu.StudentId && m.Status == 1).ToList();
            List<tb_Course> myCourseList = new List<tb_Course>();
            foreach (var item in data)
            {
                var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.CourseId);
                myCourseList.Add(cou);
            }
            return myCourseList;
        }
        public List<tb_Assignment> MyCourseAssignment(int couid)
        {
            List<tb_Assignment> assList = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            return assList;
        }
        public List<tb_Assignment> MyDeadlineList()
        {
            List<tb_Assignment> MyDeadlineList = new List<tb_Assignment>();
            var couList = MyCourseList();
            foreach (var item in couList)
            {
                var assList = MyCourseAssignment(item.Course_Id);
                foreach (var item1 in assList)
                {
                    if ((item1.Deadline - DateTime.Now) > TimeSpan.Zero)
                    {
                        if (MyDeadlineList.Contains(item1) == false)
                        {
                            MyDeadlineList.Add(item1);
                        }
                    }
                }
            }
            return MyDeadlineList;
        }
        public List<tb_News> MyNews()
        {
            var newsLists = db.tb_News.Where(m => m.To == "4" || m.To == "1").ToList();
            return newsLists;
        }

        public void ClearCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}