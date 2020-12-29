using BaiTapTuan7.Areas.Admin.Model;
using BaiTapTuan7.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Security.Cryptography;
using System.Text;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class UserController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: User
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var admin = (tb_Users)Session["user"];
            var list = db.tb_Users.ToList();
            var item = list.FirstOrDefault(m => m.Id == admin.Id);
            if (item != null)
            {
                list.Remove(item);
            }
            var lst = list.OrderBy(x => x.Usertype).ToPagedList(page, pageSize);
            return View(lst);
        }
        public ActionResult AddUser()
        {
            ViewBag.ListOfType = TypeList.GetTypeList();
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(AddUserModel us1)
        {

            if (ModelState.IsValid)
            {
                var check = db.tb_Users.FirstOrDefault(m => m.Username == us1.Username);
                if (check == null)
                {
                    tb_Users us = new tb_Users();
                    us.Username = us1.Username;
                    us.Usertype = us1.Usertype;
                    us.Email = us1.Email;
                    us.Password = MD5(us1.Password);
                    if (us1.Usertype != "admin")
                    {
                        us.Block = true;
                    }
                    else
                    {
                        us.Block = false;
                    }
                    us.RegisterDate = DateTime.Now;
                    db.tb_Users.Add(us);
                    db.SaveChanges();
                    tb_Users uz = db.tb_Users.FirstOrDefault(m => m.Username == us.Username);
                    if (uz.Usertype == "teacher" || uz.Usertype == "admin")

                    {
                        tb_Teacher tc = new tb_Teacher();
                        tc.TeacherFirstName = us1.FirstName;
                        tc.TeacherLastName = us1.LastName;
                        tc.Gmail = us1.Email;
                        tc.UserId = uz.Id;
                        db.tb_Teacher.Add(tc);
                    }
                    else if (uz.Usertype == "student")
                    {
                        tb_Student stu = new tb_Student();
                        stu.FirstName = us1.FirstName;
                        stu.LastName = us1.LastName;
                        stu.Gmail = us1.Email;
                        stu.UserId = uz.Id;
                        db.tb_Student.Add(stu);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ListOfType = TypeList.GetTypeList();
                    ModelState.AddModelError("", "Username has exists");
                    return View("AddUser", us1);
                }
            }
            else
            {
                ViewBag.ListOfType = TypeList.GetTypeList();
                return View("AddUser",us1);
            }
        }
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var us = db.tb_Users.Find(id);
                return View("EditUser", us);
            }
        }
        [HttpPost]
        public ActionResult EditUser(tb_Users tb)
        {
            if (ModelState.IsValid)
            {
                var result = db.tb_Users.Find(tb.Id);
                if (result != null)
                {
                    result.Password = MD5(tb.Password);
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Can't edit user");
                    return View("EditUser", tb);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult ActiveUser(int? Id)
        {

            if (ModelState.IsValid)
            {
                tb_Users us = db.tb_Users.Find(Id);
                if (us != null)
                {
                    us.Block = false;
                    db.SaveChanges();
                    return RedirectToAction("Index", us);
                }
                else
                {
                    return RedirectToAction("Index", us);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult UnactiveUser(int? Id)
        {

            if (ModelState.IsValid)
            {
                tb_Users us = db.tb_Users.Find(Id);
                if (us != null)
                {
                    us.Block = true;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult RemoveUser(int? Id)
        {

            if (ModelState.IsValid)
            {
                tb_Users us = db.tb_Users.Find(Id);
                if (us == null)
                {
                    return RedirectToAction("UserList");
                }
                else
                {

                    if (us.Usertype == "student")
                    {
                        return RedirectToAction("RemoveStudentByUserId", us);

                    }
                    else
                    {
                        return RedirectToAction("RemoveTeacherByUserId", new { usid = Id});
                    }
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        public ActionResult RemoveTeacherByUserId(int usid)
        {

            if (ModelState.IsValid)
            {
                var us = db.tb_Users.Find(usid);
                tb_Teacher tc = db.tb_Teacher.AsNoTracking().Where(m => m.UserId == usid).FirstOrDefault();
                db.Entry(tc).State = EntityState.Deleted;
                db.Entry(us).State = EntityState.Deleted;
                var cou = db.tb_Course.Where(m => m.TeacherId == tc.TeacherId).ToList();
                foreach (var item in cou)
                {
                    DeleteCourse(item.Course_Id);
                }
                db.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult RemoveStudentByUserId(tb_Users us)
        {
            int userid = us.Id;

            if (ModelState.IsValid)
            {
                tb_Student stu = db.tb_Student.FirstOrDefault(m => m.UserId == userid);
                DeleteStudentRe(stu.StudentId);
                db.Entry(stu).State = EntityState.Deleted;
                db.Entry(us).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public void DeleteStudentRe(int? stuid)
        {
            var studentCourse = db.tb_StudentCourse.Where(m => m.StudentId == stuid).ToList();
            var studentAssignment = db.tb_Student_Assignment.Where(m => m.Student_Id == stuid).ToList();
            var studentScore = db.tb_Score.Where(m => m.Student_id == stuid);
            foreach (var item in studentCourse)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            foreach (var item in studentAssignment)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            foreach (var item in studentScore)
            {
                db.Entry(item).State = EntityState.Deleted;
            }

        }

        public void DeleteCourse(int couid)
        {
            var cou = db.tb_Course.Find(couid);
            var cts = db.tb_StudentCourse.Where(m => m.CourseId == couid).ToList();
            var assignment = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            var content = CourseContentList(couid);
            foreach (var item in content)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            foreach (var item in assignment)
            {
                var StuAssignment = db.tb_Student_Assignment.Where(m => m.Assignment_Id == item.Assignment_Id).ToList();
                foreach (var item1 in StuAssignment)
                {
                    db.Entry(item1).State = EntityState.Deleted;
                }
                var score = db.tb_Score.Where(m => m.Assignment_id == item.Assignment_Id).ToList();
                foreach (var item2 in score)
                {
                    db.Entry(item2).State = EntityState.Deleted;
                }
                db.Entry(item).State = EntityState.Deleted;
            }
            db.Entry(cou).State = EntityState.Deleted;
            foreach (var item in cts)
            {
                var ctss = db.tb_StudentCourse.Find(item.Id);
                db.Entry(ctss).State = EntityState.Deleted;
            }
            db.SaveChanges();
        }
        public List<tb_Content> CourseContentList(int? couid)
        {
            List<tb_Content> contentList = new List<tb_Content>();
            var listContentId = db.tb_Course_Content.Where(m => m.Course_Id == couid).ToList();
            foreach (var item in contentList)
            {
                var content = db.tb_Content.Find(item.Content_Id);
                contentList.Add(content);
            }
            return contentList;
        }
        //Encrypt
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

    }
}