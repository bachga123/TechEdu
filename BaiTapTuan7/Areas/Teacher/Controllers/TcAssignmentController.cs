using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
using BaiTapTuan7.Areas.Teacher.Model;
using System.IO;

namespace BaiTapTuan7.Areas.Teacher.Controllers
{
    public class TcAssignmentController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Teacher/TcAssignment
        public ActionResult Index()
        {
            ViewBag.Teacher = (tb_Teacher)Session["teacher"];
            int couid = Convert.ToInt32(Session["couid"]);
            ViewBag.cou = db.tb_Course.Find(couid);
            List<tb_Assignment> assList = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            foreach (var item in assList)
            {
                if (item.Deadline < DateTime.Now)
                {
                    CloseAssignment(item.Assignment_Id);
                }
            }
            return View("Index", assList);
        }
        public ActionResult CreateAssignment(int couid)
        {
            ViewBag.Teacher = (tb_Teacher)Session["teacher"];
            ViewBag.cou = db.tb_Course.Find(couid);
            tb_Assignment ass = new tb_Assignment();
            ass.Course_Id = couid;
            ass.CreatedDate = DateTime.Now;
            return View("CreateAssignment", ass);
        }
        [HttpPost]
        public ActionResult CreateAssignment(tb_Assignment ass, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                tb_Assignment as1 = new tb_Assignment();
                as1.Course_Id = ass.Course_Id;
                as1.CreatedDate = ass.CreatedDate;
                if (ass.Deadline != null)
                {
                    DateTime ta = (DateTime)ass.Deadline;
                    ta = ta.AddHours(23);
                    ta = ta.AddMinutes(59);
                    ta = ta.AddSeconds(59);
                    as1.Deadline = ta;
                }
                if (file != null)
                {
                    as1.File = this.AddFileToFiles_Here(file);
                }
                if (ass.Details != null)
                {
                    as1.Details = ass.Details;
                }
                as1.Status = 1;
                db.Entry(as1).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("Index", new { couid = ass.Course_Id });
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

        public ActionResult AssignmentDetails(int assid)
        {
            ViewBag.studentAnswerList = CreateAssignmentScore(assid);
            tb_Assignment ass = db.tb_Assignment.Find(assid);
            ViewBag.Teacher = (tb_Teacher)Session["teacher"];
            ViewBag.cou = db.tb_Course.Find(ass.Course_Id);
            return View("AssignmentDetails", ass);
        }
        public ActionResult EditAssignment(int assid)
        {
            tb_Assignment ass = db.tb_Assignment.Find(assid);
            return View("EditAssignment", ass);
        }
        [HttpPost]
        public ActionResult EditAssignment(tb_Assignment ass, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var aas = db.tb_Assignment.Find(ass.Assignment_Id);
                aas.Deadline = ass.Deadline;
                if (file != null)
                {
                    aas.File = this.AddFileToFiles_Here(file);
                }
                aas.Details = ass.Details;
                db.Entry(aas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails", new { assid = ass.Assignment_Id });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult DeleteFileInAssignment(int assid)
        {
            if (ModelState.IsValid)
            {
                var aas = db.tb_Assignment.Find(assid);
                aas.File= null;
                //cần delte file trong Files_Here
                db.Entry(aas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EditAssignment", new { assid = assid });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult DeleteAssignment(int assid)
        {
            var ass = db.tb_Assignment.Find(assid);
            db.Entry(ass).State = EntityState.Deleted;
            var tsa = db.tb_Student_Assignment.Where(m => m.Assignment_Id == assid).ToList();
            foreach (var item in tsa)
            {
                tb_Student_Assignment ta = db.tb_Student_Assignment.Find(item.AS_Id);
                db.Entry(ta).State = EntityState.Deleted;
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { couid = ass.Course_Id });
        }
        public ActionResult CloseAssignment(int assid)
        {
            var ass = db.tb_Assignment.Find(assid);
            ass.Status = 2;
            db.Entry(ass).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { couid = ass.Course_Id });
        }
        // Score

        public ActionResult AddScoreToStudent(int stuid,int assid)
        {
            tb_Score sco = new tb_Score();
            sco.Student_id = stuid;
            sco.Assignment_id = assid;
            return PartialView("AddScoreToStudent", sco);
        }
        [HttpPost]
        public ActionResult AddScoreToStudent(tb_Score sco)
        {
            if (ModelState.IsValid)
            {
                if (sco.Score == null)
                {
                    return RedirectToAction("AssignmentDetails", new { assid = sco.Assignment_id });
                }
                tb_Score newsco = new tb_Score();
                newsco.Assignment_id = sco.Assignment_id;
                newsco.Student_id = sco.Student_id;
                newsco.Score = sco.Score;
                newsco.details = sco.details;
                db.Entry(newsco).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails", new { assid = sco.Assignment_id });

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public List<AssignmentScore> CreateAssignmentScore(int assid)
        {
            List<AssignmentScore> listAssignmentScore = new List<AssignmentScore>();
            var listAssignmentAnswer = db.tb_Student_Assignment.Where(m => m.Assignment_Id == assid).ToList();
            foreach (var item in listAssignmentAnswer)
            {
                tb_Student stu = db.tb_Student.Find(item.Student_Id);
                tb_Score score = db.tb_Score.FirstOrDefault(m => m.Student_id == item.Student_Id && m.Assignment_id == assid);
                AssignmentScore asc = new AssignmentScore();
                asc.StudentId = (int)item.Student_Id;
                asc.Answer = item.Decriptions;
                asc.StudentName = stu.FirstName + " " + stu.LastName;
                if (score != null)
                {
                    asc.Score = (float)score.Score;
                }
                else
                    asc.Score = -1;

                if (listAssignmentScore.Contains(asc) == false)
                {
                    listAssignmentScore.Add(asc);
                }
            }
            return listAssignmentScore;
        }
    }
}