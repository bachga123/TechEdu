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
            var listTeacher = db.tb_Teacher.OrderByDescending(m => m.TeacherFirstName).ToPagedList(page, pageSize);
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
                    var coua = db.tb_Course.Find(item.Course_Id);
                    db.Entry(coua).State = EntityState.Deleted;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}