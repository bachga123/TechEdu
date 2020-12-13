using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
using PagedList;
namespace BaiTapTuan7.Areas.Student.Controllers
{
    [AuthorizeController]
    public class StuCourseController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Student/Course
        public ActionResult Index()
        {
            return View(MyCourseList());
        }
        public ActionResult AllOfMyCourse(int page = 1,int pageSize = 10)
        {
            tb_Student stu = (tb_Student)Session["student"];
            var courseList = db.tb_Course.ToList();
            var listOfMyCourse = db.tb_StudentCourse.Where(m => m.StudentId == stu.StudentId);
            List<tb_Course> cls = new List<tb_Course>();
            foreach(var item in listOfMyCourse)
            {
                var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.CourseId);
                cls.Add(cou);
            }
            foreach (var item in cls)
            {
                var cou = db.tb_Course.Find(item.Course_Id);
                courseList.Remove(cou);
            }
            var pageList = courseList.OrderByDescending(m => m.Course_Name).ToPagedList(page, pageSize);
            return View("AllOfMyCourse",pageList);
        }
        public ActionResult EnrollToCourse(int? couid)
        {
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                tb_Student stu = (tb_Student)Session["student"];
                var cts = db.tb_Course.SingleOrDefault(m => m.Course_Id == couid);
                tb_StudentCourse cts1 = new tb_StudentCourse();
                cts1.CourseId = couid;
                cts1.StudentId = stu.StudentId;
                cts1.Status = 2;
                db.Entry(cts1).State = EntityState.Added;
                db.SaveChanges();
            }
            return RedirectToAction("AllOfMyCourse");
        }
        public ActionResult OutCourse(int? couid)
        {
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                tb_Student stu = (tb_Student)Session["student"];
                tb_StudentCourse cts = db.tb_StudentCourse.FirstOrDefault(m => m.StudentId == stu.StudentId && m.CourseId == couid && m.Status == 1);
                db.Entry(cts).State = EntityState.Deleted;
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult CourseDetails(int? id)
        {
            tb_Course cou = db.tb_Course.Find(id);
            ViewBag.Teacher = db.tb_Teacher.Find(cou.TeacherId);
            return View("CourseDetails", cou);
        }

        // Assignment
        public ActionResult CourseAssignment(int couid)
        {
            ViewBag.couid = couid;
            List<tb_Assignment> assList = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            return View("CourseAssignment", assList);
        }
        public ActionResult AssignmentDetails(int assid)
        {
            var studentAnswerList = db.tb_Student_Assignment.Where(m => m.Assignment_Id == assid);
            ViewBag.studentAnswerList = studentAnswerList;
            tb_Assignment ass = db.tb_Assignment.Find(assid);
            return View("AssignmentDetails", ass);
        }

        public ActionResult AnswerAssignment(int assid)
        {
            tb_Student stu = (tb_Student)Session["student"];
            var tsa = db.tb_Student_Assignment.FirstOrDefault(m => m.Assignment_Id == assid && m.Student_Id == stu.StudentId);
            if (tsa != null)
            {
                return RedirectToAction("EditAnswerAssignment",new { asid = tsa.Assignment_Id});
            }
            else
            {
                tsa = new tb_Student_Assignment();
                tsa.Assignment_Id = assid;
                tsa.Student_Id = stu.StudentId;
                return View("AnswerAssignment", tsa);
            }
        }
        [HttpPost]
        public ActionResult AnswerAssignment(tb_Student_Assignment tsa, HttpPostedFileBase file)
        {
            if(ModelState.IsValid)
            {
                tb_Student_Assignment a = new tb_Student_Assignment();
                a.Assignment_Id = tsa.Assignment_Id;
                a.Student_Id = tsa.Student_Id;
                a.Assignment_Id = tsa.Assignment_Id;
                if (file != null)
                {
                    a.File = this.AddFileToFiles_Here(file);
                }
                a.Decriptions = tsa.Decriptions;
                db.Entry(a).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails", new { assid = tsa.Assignment_Id });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        //add file to Files_Here
        public String AddFileToFiles_Here(HttpPostedFileBase file)
        {
            string path = Server.MapPath("~/Files_Here");
            string fileName = Path.GetFileName(file.FileName);
            //cần xử lý fullPath tránh bị trùng?
            string fullPath = Path.Combine(path, fileName);

            file.SaveAs(fullPath);
            string filePath = "Files_Here/" + fileName;
            return filePath;
        }
        public ActionResult EditAnswerAssignment(int? asid)
        {
            tb_Student stu = (tb_Student)Session["student"];
            var tsa = db.tb_Student_Assignment.FirstOrDefault(m => m.Assignment_Id == asid && m.Student_Id == stu.StudentId);
            if (tsa == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View("EditAnswerAssignment", tsa);
        }
        [HttpPost]
        public ActionResult EditAnswerAssignment(tb_Student_Assignment tsa, HttpPostedFileBase file)
        {
            var result = db.tb_Student_Assignment.FirstOrDefault(m => m.Assignment_Id == tsa.Assignment_Id && m.Student_Id == tsa.Student_Id);
            if(result != null && ModelState.IsValid)
            {
                if (file != null)
                {
                    result.File = this.AddFileToFiles_Here(file);
                }
                result.Decriptions = tsa.Decriptions;
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails", new { assid = tsa.Assignment_Id });
            }else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public List<tb_Course> MyCourseList()
        {
            tb_Student stu = (tb_Student)Session["student"];
            var data = db.tb_StudentCourse.Where(m => m.StudentId == stu.StudentId && m.Status == 1).ToList();
            List<tb_Course> myCourseList = new List<tb_Course>();
            foreach (var item in data)
            {
                var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.CourseId );
                myCourseList.Add(cou);
            }
            return myCourseList;
        }
    }
}