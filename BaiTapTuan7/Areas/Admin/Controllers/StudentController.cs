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
        public ActionResult Index(int page = 1,int pageSize = 10)
        {

            var listStudent = db.tb_Student.OrderByDescending(m => m.FirstName).ToPagedList(page,pageSize);
            return View(listStudent);
        }
        public ActionResult EditStudent(int id)
        {
            tb_Student stu = db.tb_Student.FirstOrDefault(m => m.UserId == id);
            return View("EditStudent", stu);
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
                        var new_stu = new tb_Student();
                        new_stu.FirstName = stu.FirstName;
                        new_stu.LastName = stu.LastName;
                        new_stu.Gmail = stu.Gmail;
                        new_stu.PhoneNumber = stu.PhoneNumber;
                        new_stu.DateOfBirth = stu.DateOfBirth;
                        new_stu.PlaceOfBirth = stu.PlaceOfBirth;
                        HttpPostedFileBase upload = Request.Files["image"]; 
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            new_stu.Images = binaryReader.ReadBytes(upload.ContentLength);
                        db.Entry(new_stu).State = EntityState.Added;
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}