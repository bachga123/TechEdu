using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BaiTapTuan7.Models;
namespace BaiTapTuan7.Controllers
{
    [AuthorizeController]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            using(TechEduEntities db = new TechEduEntities())
            {
                List<tb_Users> lst = db.tb_Users.ToList();
                ViewBag.UsersList = lst;
            }
            return View();
        }
        public ActionResult AddUser()
        {
            ViewBag.ListOfType = TypeList.GetTypeList();
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(tb_Users us)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.tb_Users.FirstOrDefault(m => m.Username == us.Username);
                    if (check == null)
                    {
                        us.Block = true;
                        us.RegisterDate = DateTime.Now;
                        db.tb_Users.Add(us);
                        db.SaveChanges();
                        tb_Users uz = db.tb_Users.FirstOrDefault(m => m.Username == us.Username);
                        if (uz.Usertype == "teacher")
                        {
                            tb_Teacher tc = new tb_Teacher();
                            tc.UserId = uz.Id;
                            db.tb_Teacher.Add(tc);
                        }
                        else if (uz.Usertype == "student")
                        {
                            tb_Student stu = new tb_Student();
                            stu.UserId = uz.Id;
                            db.tb_Student.Add(stu);
                        }
                        db.SaveChanges();
                        return RedirectToAction("EditUser", uz);
                    }
                    else
                    {
                        us.AddUserError = "Username has exists";
                        return View("AddUser", us);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        [HttpGet]
        public ActionResult UserList()
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                List<tb_Users> listUsers = db.tb_Users.ToList();
                ViewBag.UserList = listUsers;
            }
            return View();
        }
        public ActionResult TeacherList()
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                List<tb_Teacher> listTeacher = db.tb_Teacher.ToList();
                ViewBag.TeacherList = listTeacher;
            }
            return View();
        }
        public ActionResult StudentList()
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                List<tb_Student> listStudent = db.tb_Student.ToList();
                ViewBag.StudentList = listStudent;
            }
            return View();
        }
        public ActionResult ActiveUser(int? Id)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_Users us = db.tb_Users.Find(Id);
                    if (us != null)
                    {
                        us.Block = false;
                        db.SaveChanges();
                        return RedirectToAction("UserList",us);
                    }
                    else
                    {
                        return RedirectToAction("UserList",us);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult UnactiveUser(int? Id)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_Users us = db.tb_Users.Find(Id);
                    if (us != null)
                    {
                        us.Block = true;
                        db.SaveChanges();
                        return RedirectToAction("UserList");
                    }
                    else
                    {
                        return RedirectToAction("UserList");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult RemoveUser(int? Id)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if(ModelState.IsValid)
                {
                    tb_Users us = db.tb_Users.Find(Id);
                    if(us == null)
                    {
                        return RedirectToAction("UserList");
                    }
                    else
                    {
                        if(us.Usertype == "teacher")
                        {
                            return RedirectToAction("RemoveTeacherByUserId",us);
                        }
                        else
                        {
                            return RedirectToAction("RemoveStudentByUserId",us);
                        }
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult RemoveTeacherByUserId(tb_Users us)
        {
            int userid = us.Id;
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == userid);
                    db.Entry(tc).State = EntityState.Deleted;
                    db.Entry(us).State = EntityState.Deleted;
                    db.SaveChanges();
                    return RedirectToAction("UserList", "Admin");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult RemoveStudentByUserId(tb_Users us)
        {
            int userid = us.Id;
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_Student tc = db.tb_Student.FirstOrDefault(m => m.UserId ==userid);
                    db.Entry(tc).State = EntityState.Deleted;
                    db.Entry(us).State = EntityState.Deleted;
                    db.SaveChanges();
                    return RedirectToAction("UserList", "Admin");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult EditUser(int? Id)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                tb_Users us = db.tb_Users.Find(Id);
                if(us.Usertype == "teacher")
                {
                    tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == Id);
                    return View("EditTeacher", tc);
                }
                else if (us.Usertype == "student")
                {
                    tb_Student stu = db.tb_Student.FirstOrDefault(m => m.UserId == Id);
                    return View("EditStudent", stu);
                }
                return View("UserList");
            }
        }
        [HttpPost]
        public ActionResult EditTeacher(tb_Teacher tc)
        {
            using(TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.tb_Teacher.FirstOrDefault(m => m.UserId == tc.UserId);
                    if (check != null)
                    {
                        tb_Teacher oldteach = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == check.TeacherId);
                        db.Entry(oldteach).State = EntityState.Deleted;
                        db.Entry(tc).State = EntityState.Added;
                        db.SaveChanges();
                        return RedirectToAction("UserList","Admin");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        [HttpPost]
        public ActionResult EditStudent(tb_Student stu)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.tb_Student.FirstOrDefault(m => m.UserId == stu.UserId);
                    if (check != null)
                    {
                        tb_Student oldstu = db.tb_Student.FirstOrDefault(m => m.StudentId == check.StudentId);
                        db.Entry(oldstu).State = EntityState.Deleted;
                        db.Entry(stu).State = EntityState.Added;
                        db.SaveChanges();
                        return RedirectToAction("UserList", "Admin");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}