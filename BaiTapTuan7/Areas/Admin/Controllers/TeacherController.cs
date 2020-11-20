using BaiTapTuan7.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class TeacherController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Teacher
        public ActionResult Index()
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                List<tb_Teacher> listTeacher = db.tb_Teacher.ToList();
                ViewBag.TeacherList = listTeacher;
            }
            return View();
        }
        public ActionResult EditTeacher(int id)
        {
            tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == id);
            return View("EditTeacher",tc);
        }
        [HttpPost]
        public ActionResult EditTeacher(tb_Teacher tc)
        {
            using (TechEduEntities db = new TechEduEntities())
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