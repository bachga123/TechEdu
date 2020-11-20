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
namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
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