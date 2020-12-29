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

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class StudentController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Student
        public ActionResult Index(int page = 1, int pageSize = 10)
        {

            var listStudent = db.tb_Student.OrderByDescending(m => m.FirstName).ToPagedList(page, pageSize);
            return View(listStudent);
        }
        public ActionResult Details(int id)
        {
            var stu = db.tb_Student.Find(id);
            if (stu == null)
                return new HttpNotFoundResult();
            return View(stu);
        }
        public ActionResult EditStudent(int id)
        {
            tb_Student stu = db.tb_Student.FirstOrDefault(m => m.StudentId == id);
            return View("EditStudent", stu);
        }
        [HttpPost]
        public ActionResult EditStudent(tb_Student stu)
        {
            if (ModelState.IsValid)
            {
                tb_Student oldstu = db.tb_Student.FirstOrDefault(m => m.StudentId == stu.StudentId);
                if (oldstu != null)
                {
                    oldstu.FirstName = stu.FirstName;
                    oldstu.LastName = stu.LastName;
                    oldstu.Gmail = stu.Gmail;
                    oldstu.PhoneNumber = stu.PhoneNumber;
                    oldstu.DateOfBirth = stu.DateOfBirth;
                    oldstu.PlaceOfBirth = stu.PlaceOfBirth;
                    HttpPostedFileBase upload = Request.Files["image"];
                    if (upload.FileName != "")
                    {
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            oldstu.Images = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    db.Entry(oldstu).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Student");
                }
                else
                {
                    ModelState.AddModelError("", "Edit Student failed");
                    return View("EditStudent", stu);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit Student failed");
                return View("EditStudent", stu);
            }
        }

        public ActionResult Delete(int? stuid)
        {
            if (stuid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var stu = db.tb_Student.FirstOrDefault(m => m.StudentId == stuid);
                var us = db.tb_Users.Find(stu.UserId);
                var cts = db.tb_StudentCourse.Where(m => m.StudentId == stuid).ToList();
                db.Entry(us).State = EntityState.Deleted;
                db.Entry(stu).State = EntityState.Deleted;
                DeleteStudentRe(stuid);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        public void DeleteStudentRe(int? stuid)
        {
            var studentCourse = db.tb_StudentCourse.Where(m => m.StudentId == stuid).ToList();
            var studentAssignment = db.tb_Student_Assignment.Where(m => m.Student_Id == stuid).ToList();
            var studentScore = db.tb_Score.Where(m => m.Student_id == stuid);
            foreach(var item in studentCourse)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            foreach (var item in studentAssignment)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            foreach (var item in studentScore)
            {
                db.Entry(item).State = EntityState.Deleted;
            }

        }
    }
}