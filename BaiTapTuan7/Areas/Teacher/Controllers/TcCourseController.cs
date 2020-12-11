using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
using PagedList;
namespace BaiTapTuan7.Areas.Teacher.Controllers
{
    [AuthorizeController]
    public class TcCourseController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Teacher/Course
        public ActionResult Index()
        {
            return View(MyCourseList());
        }
        public ActionResult EnrollStudentToCourse(int stu, int cou)
        {
            return View();
        }
        public ActionResult RemoveStudentFromCourse(int stu, int cou)
        {
            return View();
        }
        public ActionResult CourseDetails(int couid)
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            ViewBag.Teacher = tc;
            Session["couid"] = couid;
            tb_Course cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == couid && m.TeacherId == tc.TeacherId);
            return View("CourseDetails", cou);
        }
        public ActionResult EditCourseDetails()
        {
            int couid = Convert.ToInt32(Session["couid"]);
            tb_Course cou = db.tb_Course.Find(couid);
            return View("EditCourseDetails",cou);
        }
        [HttpPost]
        public ActionResult EditCourseDetails(tb_Course cou)
        {
            var result = db.tb_Course.Find(cou.Course_Id);
            if(ModelState.IsValid)
            {
                result.Decription = cou.Decription;
                result.Details = cou.Details;
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CourseDetails", new { couid = cou.Course_Id });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult StudentOnCourse(int page = 1,int pageSize =2)
        {
            int couid = Convert.ToInt32(Session["couid"]);
            ViewBag.couid = couid;
            var stuInCourse = db.tb_CTS.Where(m => m.CourseId == couid);
            List<tb_Student> stuList = new List<tb_Student>();
            foreach(var item in stuInCourse)
            {
                var stu = db.tb_Student.FirstOrDefault(m => m.StudentId == item.StudentId);
                if(stu != null && stuList.Contains(stu) == false)
                {
                    stuList.Add(stu);
                }
            }
            return View("StudentOnCourse",stuList.ToPagedList(page,pageSize));
        }
        public ActionResult OpenNewCourse()
        {
            return View();
        }
        public ActionResult DeleteCourse()
        {
            return View();
        }
        public ActionResult StudentScore()
        {
            return View();
        }
        public List<tb_Course> MyCourseList()
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            var data = db.tb_Course.Where(m => m.TeacherId == tc.TeacherId);
            List<tb_Course> myCourseList = new List<tb_Course>();
            foreach (var item in data)
            {
                var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.Course_Id && m.TeacherId == tc.TeacherId);
                if(myCourseList.Contains(cou) == false)
                    myCourseList.Add(cou);
            }
            return myCourseList;
        }
    }
}