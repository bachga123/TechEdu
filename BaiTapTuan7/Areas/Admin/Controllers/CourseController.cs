using BaiTapTuan7.Models;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BaiTapTuan7.Areas.Admin.Model;
using System;
using System.Threading.Tasks;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class CourseController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        SendEmail email = new SendEmail();

        GetInfor inf = new GetInfor();
        // GET: Admin/Course
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var CourseList = db.tb_Course.OrderByDescending(m => m.Course_Id).ToPagedList(page, pageSize);
            ViewBag.TeacherList = db.tb_Teacher.ToList();
            return View(CourseList);
        }

        public ActionResult CreateCourse()
        {
            var list = new SelectList(TeacherSelectList(), "value", "text");
            ViewBag.teacherList = list;
            return View();
        }
        [HttpPost]
        public ActionResult CreateCourse(tb_Course cou)
        {
            var list = new SelectList(TeacherSelectList(), "value", "text");
            ViewBag.teacherList = list;
            if (ModelState.IsValid)
            {
                var check = db.tb_Course.FirstOrDefault(m => m.Course_Name == cou.Course_Name);
                if (check == null)
                {
                    var newCou = new tb_Course();
                    newCou.TeacherId = cou.TeacherId;
                    newCou.Course_Name = cou.Course_Name;
                    newCou.Decription = cou.Decription;
                    newCou.Details = cou.Details;
                    newCou.Course_Price = cou.Course_Price;
                    db.Entry(newCou).State = EntityState.Added;
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
                return View("CreateCourse",cou);
            }
        }
        public ActionResult DeleteCourse(int? couid)
        {
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                var cou = db.tb_Course.Find(couid);
                var cts = db.tb_StudentCourse.Where(m => m.CourseId == couid).ToList();
                var assignment = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
                var content = CourseContentList(couid);
                foreach(var item in content)
                {
                    db.Entry(item).State = EntityState.Deleted;
                }
                foreach(var item in assignment)
                {
                    var StuAssignment = db.tb_Student_Assignment.Where(m => m.Assignment_Id == item.Assignment_Id).ToList();
                    foreach(var item1 in StuAssignment)
                    {
                        db.Entry(item1).State = EntityState.Deleted;
                    }
                    var score = db.tb_Score.Where(m => m.Assignment_id == item.Assignment_Id).ToList();
                    foreach(var item2 in score)
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
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult EditCourse(int couid)
        {
            var check = db.tb_Course.Find(couid);
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
                    result.Course_Price = cou.Course_Price;
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
            var data = db.tb_StudentCourse.Where(m => m.CourseId == id && m.Status == 1).ToList();
            List<tb_Student> studentInCourseList = new List<tb_Student>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var stu = db.tb_Student.FirstOrDefault(m => m.StudentId == item.StudentId);
                    studentInCourseList.Add(stu);
                }
                ViewBag.studentInCourseList = studentInCourseList;
            }
            List<tb_Student> studentWantToEnroll = new List<tb_Student>();
            var data2 = db.tb_StudentCourse.Where(m => m.CourseId == id && m.Status == 2).ToList();
            if (data2 != null)
            {
                foreach (var item in data2)
                {
                    var stu = db.tb_Student.FirstOrDefault(m => m.StudentId == item.StudentId);
                    studentWantToEnroll.Add(stu);
                }
                ViewBag.studentWantToEnroll = studentWantToEnroll;
            }
            var studentNotInCourseList = db.tb_Student.ToList();
            if (studentInCourseList != null)
            {
                foreach (var item in studentInCourseList)
                {
                    var itemRemove = studentNotInCourseList.Find(m => m.StudentId == item.StudentId);
                    studentNotInCourseList.Remove(itemRemove);
                }
            }
            if (studentWantToEnroll != null)
            {
                foreach (var item in studentWantToEnroll)
                {
                    var itemRemove = studentNotInCourseList.Find(m => m.StudentId == item.StudentId);
                    studentNotInCourseList.Remove(itemRemove);
                }
            }   
            ViewBag.studentList = studentNotInCourseList;
            return View(result);
        }
        //public ActionResult EnrollStudentWantToEnroll(int stuid, int couid)
        //{
        //    var cts = db.tb_StudentCourse.FirstOrDefault(m => m.CourseId == couid && m.StudentId == stuid);
        //    cts.Status = 1;
        //    cts.
        //    db.Entry(cts).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("Details", new { id = couid });
        //}
        //public ActionResult DeleteStudentWantToEnroll(int stuid, int couid)
        //{
        //    var cts = db.tb_StudentCourse.FirstOrDefault(m => m.CourseId == couid && m.StudentId == stuid);
        //    db.Entry(cts).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return RedirectToAction("Details", new { id = couid });
        //}
        [HttpPost]
        public ActionResult EnrollStudentToCourse(FormCollection f)
        {
            int? couid = int.Parse(Request.Form["Course_Id"]);
            var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == couid);
            string[] StudentIdList = f["StudentId"].Split(new char[] { ',' });
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foreach (string id in StudentIdList)
            {
                tb_StudentCourse cts = new tb_StudentCourse();
                cts.StudentId = int.Parse(id);
                cts.CourseId = cou.Course_Id;
                cts.Status = 1;
                cts.Payment = true;
                db.Entry(cts).State = EntityState.Added;
                email.EnrollStudentSuccessMail(inf.GetStudent(int.Parse(id)).Gmail, inf.GetCourse((int)couid).Course_Name);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = couid });
        }
        public ActionResult RemoveStudent(int? stuid, int? couid)
        {
            if (stuid == null || couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var cts = db.tb_StudentCourse.SingleOrDefault(m => m.CourseId == couid && m.StudentId == stuid);
                db.Entry(cts).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = couid });
            }
        }
        public List<SelectListItem> TeacherSelectList()
        {
            List<SelectListItem> tsl = new List<SelectListItem>();
            var tcList = ListOfTeacher();
            foreach (var item in tcList)
            {
                SelectListItem tc = new SelectListItem();
                tc.Value = item.TeacherId.ToString();
                tc.Text = item.TeacherFirstName + " " + item.TeacherLastName;
                tsl.Add(tc);
            }
            return tsl;
        }
        public List<tb_Teacher> ListOfTeacher()
        {
            List<tb_Teacher> tcList = db.tb_Teacher.ToList();
            var adminList = db.tb_Users.Where(m => m.Usertype == "admin").ToList();
            foreach (var item in adminList)
            {
                tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == item.Id);
                if (tcList.Contains(tc))
                {
                    tcList.Remove(tc);
                }
            }
            return tcList;
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
    }
}
