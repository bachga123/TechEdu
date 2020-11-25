using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
using PagedList;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    public class CourseController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Admin/Course
        public ActionResult Index(int page = 1,int pageSize = 10)
        {
            var CourseList = db.tb_Course.OrderByDescending(m => m.Course_Id).ToPagedList(page, pageSize);
            return View(CourseList);
        }

        public ActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCourse(tb_Course cou)
        {
            if (ModelState.IsValid)
            {
                var check = db.tb_Course.FirstOrDefault(m => m.Title == cou.Title);
                if(check == null)
                {
                    db.Entry(cou).State = EntityState.Added;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Title has exists");
                    return View("CreateCourse", cou);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult DeleteCourse()

        {
            return View();
        }
        public ActionResult EditCourse(int id)
        {
            var check = db.tb_Course.Find(id);
            if(check != null)
            {
                return View("EditCourse", check);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult EditCourse(tb_Course cou)
        {
            if (ModelState.IsValid)
            {
                var result = db.tb_Course.FirstOrDefault(m => m.Course_Id == cou.Course_Id);
                if (result != null)
                {
                    result.Title = cou.Title;
                    result.Decription = cou.Decription;
                    result.Details = cou.Details;
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Edit Coures failed");
                    return View("EditCourse", cou);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult Details(int id)
        {
            var result = db.tb_Course.Find(id);
            if(result == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View(result);
        }
    }
}