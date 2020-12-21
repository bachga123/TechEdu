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
        public ActionResult CourseDetails(int couid)
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            ViewBag.Teacher = tc;
            Session["couid"] = couid;
            ViewBag.contentList = MyContentList(couid);
            tb_Course cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == couid && m.TeacherId == tc.TeacherId);
            return View("CourseDetails", cou);
        }
        public ActionResult EditCourseDetails()
        {
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            ViewBag.Teacher = tc;
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
            tb_Teacher tc = (tb_Teacher)Session["teacher"];
            ViewBag.Teacher = tc;
            int couid = Convert.ToInt32(Session["couid"]);
            ViewBag.cou = db.tb_Course.Find(couid);
            var stuInCourse = db.tb_StudentCourse.Where(m => m.CourseId == couid);
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
        // Thêm vào một content trong course
        public ActionResult AddContent()
        {
            tb_Content newCon = new tb_Content();
            return PartialView("AddContent",newCon);
        }
        [HttpPost]
        public ActionResult AddContent(tb_Content newCon, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                tb_Content con = new tb_Content();
                con.Title = newCon.Title;
                con.Description = newCon.Description;
                if (file != null)
                {
                    con.File = this.AddFileToFiles_Here(file);
                }
                db.Entry(con).State = EntityState.Added;
                db.SaveChanges();
                tb_Course_Content tcc = new tb_Course_Content();
                tcc.Content_Id = con.Content_Id;
                tcc.Course_Id = (int)Session["couid"];
                db.Entry(tcc).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("CourseDetails", new { couid = tcc.Course_Id });
            }
            else
            {
                ModelState.AddModelError("", "False");
                return View("AddContent", newCon);
            }
        }
        public ActionResult EditContent(int conid)
        {
            var con = db.tb_Content.Find(conid);
            return PartialView("EditContent", con);
        }
        [HttpPost]
        public ActionResult EditContent(tb_Content con, HttpPostedFileBase file)
        {
            int couid = (int)Session["couid"];
            if(ModelState.IsValid)
            {
                tb_Content tc = db.tb_Content.Find(con.Content_Id);
                tc.Description = con.Description;
                tc.Title = con.Title;
                if (file != null)
                {
                    tc.File = this.AddFileToFiles_Here(file);
                }
                db.Entry(tc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CourseDetails", new { couid = couid });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult DeleteContent(int conid)
        {
            int couid = (int)Session["couid"];
            if (ModelState.IsValid)
            {
                var con = db.tb_Content.Find(conid);
                var ccon = db.tb_Course_Content.FirstOrDefault(m => m.Content_Id == conid);
                db.Entry(con).State = EntityState.Deleted;
                db.Entry(ccon).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("CourseDetails", new { couid = couid });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult DeleteFileInContent(int conid)
        {
            if (ModelState.IsValid)
            {
                int couid = (int)Session["couid"];
                var aas = db.tb_Content.Find(conid);
                aas.File = null;
                //cần delte file trong Files_Here
                db.Entry(aas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CourseDetails", new { couid = couid });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
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
        public List<tb_Content> MyContentList(int couid)
        {
            List<tb_Course_Content> tcc = db.tb_Course_Content.Where(m => m.Course_Id == couid).ToList();
            List<tb_Content> tc = new List<tb_Content>();
            foreach(var item in tcc)
            {
                tb_Content tc1 = db.tb_Content.Find(item.Content_Id);
                if(tc.Contains(tc1) == false)
                {
                    tc.Add(tc1);
                }
            }
            return tc;
        }
    }
}