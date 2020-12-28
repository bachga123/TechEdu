using BaiTapTuan7.Areas.Admin.Model;
using BaiTapTuan7.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Security.Cryptography;
using System.Text;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    //[AuthorizeController]
    public class ThongKeController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        public ActionResult ChartOfIncomeUSD()
        {
            return View();
        }
        public ActionResult ThongKeUserType()
        {
            var admin = (tb_Users)Session["user"];
            var list = db.tb_Users.ToList();
            var item = list.FirstOrDefault(m => m.Id == admin.Id);
            var model = new List<ThongKeViewModel>();
            if (item != null)
            {
                list.Remove(item);

                var tb_UserType = db.tb_UserType.ToList();
                var tb_Users = db.tb_Users.ToList();
                foreach (var usertype in tb_UserType)
                {
                    var thongKeViewModel = new ThongKeViewModel();
                    thongKeViewModel.UserType = usertype.UserTypeName;
                    thongKeViewModel.UserCount = tb_Users.Where(x => x.Usertype == usertype.UserTypeId).Count();
                    model.Add(thongKeViewModel);
                }
            }
            return View(model);
        }

        public ActionResult ChartRender()
        {
            ViewBag.inCome = CreateListIncomeUsdOfYear(2020);
            string a = "";
            a = a + ViewBag.inCome[0];
            for (int i = 1; i < 12; i++)
            {
                a = a + "," + ViewBag.inCome[i];
            }
            ViewBag.inn = a;
            return PartialView();
        }
        public int[] CreateListIncomeUsdOfYear(int year, int month = 0)
        {
            int[] incomeMonthList = new int[12];
            if (month == 0)
            {
                for (int i = 1; i < 13; i++)
                {
                    decimal income = IncomeUsdForAllCourseIn1Month(year, i);
                    incomeMonthList[i - 1] = (int)income;
                }
            }
            else
            {
                incomeMonthList = new int[1];
                incomeMonthList[0] = (int)IncomeUsdForAllCourseIn1Month(year, month);
            }
            return incomeMonthList;
        }
        public decimal IncomeUsdForAllCourseIn1Month(int year, int month = 0)
        {
            decimal sum = 0;
            var allOfCourse = db.tb_Course.Where(m => m.Course_Price != null || m.Course_Price != 0).ToList();
            foreach (var item in allOfCourse)
            {
                int count = IncomeStudentFor1CourseIn1Month(item.Course_Id, year, month);
                sum = sum + (decimal)(count * item.Course_Price);
            }
            return sum;
        }
        public int IncomeStudentFor1CourseIn1Month(int couid, int year, int month = 0)
        {
            var listOfIncomeStudent = listOfIncomeStudentIn1Month(couid, year, month);
            return listOfIncomeStudent.Count;
        }
        public List<tb_StudentCourse> listOfIncomeStudentIn1Month(int couid, int year, int month = 0)
        {
            if (month != 0)
            {
                return db.tb_StudentCourse.Where(m => m.CourseId == couid && ((DateTime)m.EnrollDate).Month == month && ((DateTime)m.EnrollDate).Year == year).ToList();
            }
            else
                return db.tb_StudentCourse.Where(m => m.CourseId == couid && ((DateTime)m.EnrollDate).Year == year).ToList();

        }

    }

   
}