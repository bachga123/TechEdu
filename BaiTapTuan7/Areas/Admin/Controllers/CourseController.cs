using BaiTapTuan7.Models;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class CourseController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Admin/Course
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var CourseList = db.tb_Course.OrderByDescending(m => m.Course_Id).ToPagedList(page, pageSize);
            ViewBag.TeacherList = db.tb_Teacher.ToList();
            return View(CourseList);
        }

        public ActionResult CreateCourse()
        {
            var list = new SelectList(db.tb_Teacher, "TeacherId", "TeacherLastName");
            ViewBag.teacherList = list;
            return View();
        }
        [HttpPost]
        public ActionResult CreateCourse(tb_Course cou)
        {
            if (ModelState.IsValid)
            {
                var check = db.tb_Course.FirstOrDefault(m => m.Course_Name == cou.Course_Name);
                if (check == null)
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
            if (check != null)
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
                    result.Course_Name = cou.Course_Name;
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
        public ActionResult Details(int? id)
        {
            var result = db.tb_Course.Find(id);
            if (result == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.teacher = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == result.TeacherId);
            var data = db.tb_CTS.Where(m => m.CourseId == id).ToList();
            List<tb_Student> studentInCourseList = new List<tb_Student>();
            foreach(var item in data)
            {
                var stu = db.tb_Student.FirstOrDefault(m => m.StudentId == item.StudentId);
                studentInCourseList.Add(stu);
            }
            ViewBag.studentInCourseList = studentInCourseList;
            var studentNotInCourseList = db.tb_Student.ToList();
            foreach (var item in studentInCourseList)
            {
                var itemRemove = studentNotInCourseList.Find(m => m.StudentId == item.StudentId);
                studentNotInCourseList.Remove(itemRemove);
            }
            ViewBag.studentList = studentNotInCourseList;
            return View(result);
        }
        //public ActionResult EnrollStudentToCourse(int? couid, int? stuid)
        //{
        //    if (couid == null || stuid == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == couid);
        //        tb_CTS cts = new tb_CTS();
        //        cts.StudentId = stuid;
        //        cts.CourseId = cou.Course_Id;
        //        cts.TeacherId = cou.TeacherId;
        //        db.Entry(cts).State = EntityState.Added;
        //        db.SaveChanges();
        //        return RedirectToAction("Details", new { id = couid });
        //    }
        //    else
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //}

        [HttpPost]
        public ActionResult EnrollStudentToCourse(FormCollection f)
        {
            int? couid = int.Parse(Request.Form["Course_Id"]);
            var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == couid);
            string[] StudentIdList = f["StudentId"].Split(new char[] { ',' });
            if(couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foreach(string id in StudentIdList)
            {
                tb_CTS cts = new tb_CTS();
                cts.StudentId = int.Parse(id);
                cts.CourseId = cou.Course_Id;
                cts.TeacherId = cou.TeacherId;
                db.Entry(cts).State = EntityState.Added;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = couid });
        }
    }
}
