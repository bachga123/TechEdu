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
        public ActionResult Index(int page = 1,int pageSize = 10)
        {
            var lst = db.tb_Users.OrderBy(x => x.Usertype).ToPagedList(page, pageSize);
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
                    if (uz.Usertype == "teacher"  || uz.Usertype == "admin")

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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult EditUser(int? id)
        {
            if(id == null)
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
            if(ModelState.IsValid)
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
                    if (us.Usertype == "teacher")
                    {
                        return RedirectToAction("RemoveTeacherByUserId", us);
                    }
                    else
                    {
                        return RedirectToAction("RemoveStudentByUserId", us);
                    }
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult RemoveTeacherByUserId(tb_Users us)
        {
            int userid = us.Id;

            if (ModelState.IsValid)
            {
                tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == userid);
                db.Entry(tc).State = EntityState.Deleted;
                db.Entry(us).State = EntityState.Deleted;
                var cou = db.tb_Course.Where(m => m.TeacherId == tc.TeacherId).ToList();
                foreach (var item in cou)
                {
                    var coua = db.tb_Course.Find(item.Course_Id);
                    db.Entry(coua).State = EntityState.Deleted;
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
                tb_Student tc = db.tb_Student.FirstOrDefault(m => m.UserId == userid);
                db.Entry(tc).State = EntityState.Deleted;
                db.Entry(us).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
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