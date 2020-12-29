using BaiTapTuan7.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class TeacherController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Teacher
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var admin = (tb_Teacher)Session["admin"];
            var list = ListOfAdminTeacher();
            var listTeacher = list.OrderByDescending(m => m.TeacherFirstName).ToPagedList(page, pageSize);
            return View(listTeacher);
        }
        public ActionResult EditTeacher(int id)
        {
            tb_Teacher tc = db.tb_Teacher.Find(id);
            return View("EditTeacher",tc);
        }
        [HttpPost]
        public ActionResult EditTeacher(tb_Teacher tc)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_Teacher Result = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == tc.TeacherId);
                    if (Result != null)
                    {
                        Result.TeacherFirstName = tc.TeacherFirstName;
                        Result.TeacherLastName = tc.TeacherLastName;
                        Result.Gmail = tc.Gmail;
                        Result.DateOfBirth = tc.DateOfBirth;
                        Result.PlaceOfBirth = tc.PlaceOfBirth;
                        Result.PhoneNumber = tc.PhoneNumber;
                        Result.Rank = tc.Rank;
                        HttpPostedFileBase upload = Request.Files["image"];
                        if (upload.FileName != "")
                        {
                            using (var binaryReader = new BinaryReader(upload.InputStream))
                                Result.Images = binaryReader.ReadBytes(upload.ContentLength);
                        }
                        db.Entry(Result).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Teacher");
                    }
                    else
                    {
                        ModelState.AddModelError("","Edit Teacher failed");
                        return View("EditTeacher",tc);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
        public ActionResult Delete(int? tcid)
        {
            if (tcid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var tc = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == tcid);
                var us = db.tb_Users.Find(tc.UserId);
                var cou = db.tb_Course.Where(m => m.TeacherId == tcid).ToList();
                db.Entry(us).State = EntityState.Deleted;
                db.Entry(tc).State = EntityState.Deleted;
                foreach(var item in cou)
                {
                    DeleteCourse(item.Course_Id);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        public void DeleteCourse(int couid)
        {
            var cou = db.tb_Course.Find(couid);
            var cts = db.tb_StudentCourse.Where(m => m.CourseId == couid).ToList();
            var assignment = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            var content = CourseContentList(couid);
            foreach (var item in content)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            foreach (var item in assignment)
            {
                var StuAssignment = db.tb_Student_Assignment.Where(m => m.Assignment_Id == item.Assignment_Id).ToList();
                foreach (var item1 in StuAssignment)
                {
                    db.Entry(item1).State = EntityState.Deleted;
                }
                var score = db.tb_Score.Where(m => m.Assignment_id == item.Assignment_Id).ToList();
                foreach (var item2 in score)
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
        public List<tb_Teacher> ListOfAdminTeacher()
        {
            List<tb_Teacher> tcList = db.tb_Teacher.ToList();
            var adminList = db.tb_Users.Where(m => m.Usertype == "admin").ToList();
            foreach(var item in adminList)
            {
                tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == item.Id);
                if(tcList.Contains(tc))
                {
                    tcList.Remove(tc);
                }
            }
            return tcList;
        }
    }
}