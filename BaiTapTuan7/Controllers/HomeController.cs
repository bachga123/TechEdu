using BaiTapTuan7.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BaiTapTuan7.Controllers
{
    public class HomeController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        SendEmail mail = new SendEmail();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangNhap()
        {
            ViewBag.Message = "Login";
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string taikhoan = f["username"].ToString();
            string matkhau = MD5(f["password"].ToString());
            //string matkhaumd5 = GetMD5(matkhau);
            tb_Users us = db.tb_Users.SingleOrDefault(n => n.Username == taikhoan && n.Password == matkhau);
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                if (us.Block == true)
                {
                    return Content("er_block");
                }
                Session["user"] = us;
                Session["userName"] = us.Username;

                if (us.Usertype == "admin")
                {
                    Session["admin"] = db.tb_Teacher.FirstOrDefault(m => m.UserId == us.Id);
                    return Content("/Admin");
                }
                else if (us.Usertype == "student")
                {
                    Session["student"] = db.tb_Student.FirstOrDefault(m => m.UserId == us.Id);
                    return Content("/Student");
                }
                else if (us.Usertype == "teacher")
                {
                    Session["teacher"] = db.tb_Teacher.FirstOrDefault(m => m.UserId == us.Id);
                    return Content("/Teacher");
                }
                else
                {
                    return Content("false");
                }
            }
            else
            {
                return Content("false");
            }
        }
        public ActionResult Register()
        {
            RegisterModel us = new RegisterModel();
            return View("Register", us);
        }
        [HttpPost]
        public ActionResult Register(RegisterModel us1)
        {
            if (ModelState.IsValid)
            {
                var check = db.tb_Users.FirstOrDefault(m => m.Username == us1.UserName || m.Email == us1.Gmail);
                if (check == null)
                {
                    if (us1.Password == us1.RepeatPassword)
                    {
                        Guid activeCode = Guid.NewGuid();
                        tb_Users us = new tb_Users();
                        us.Username = us1.UserName;
                        us.Usertype = "student";
                        us.Email = us1.Gmail;
                        us.Password = MD5(us1.Password);
                        us.Block = true;
                        us.RegisterDate = DateTime.Now;
                        us.ActivateCode = activeCode;
                        db.tb_Users.Add(us);
                        db.SaveChanges();
                        tb_Student stu = new tb_Student();
                        stu.FirstName = us1.Gmail;
                        stu.LastName = us1.LastName;
                        stu.Gmail = us1.Gmail;
                        stu.UserId = us.Id;
                        db.tb_Student.Add(stu);
                        db.SaveChanges();

                        mail.SendVerificationLinkMail(us.Email, activeCode.ToString());
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ListOfType = TypeList.GetTypeList();
                        ModelState.AddModelError("", "Password repeat not same");
                        return View("Register", us1);
                    }
                }
                else
                {
                    ViewBag.ListOfType = TypeList.GetTypeList();
                    ModelState.AddModelError("", "Username Or Email has exists");
                    return View("Register", us1);
                }
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("ActiveAccount/{id ?}")]
        public ActionResult ActiveAccount(string id)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var value = db.tb_Users.Where(m => m.ActivateCode == new Guid(id)).FirstOrDefault();
            if (value != null)
            {
                value.Block = false;
                db.SaveChanges();
                mail.ActiveAccount(value.Email, "Dear user, Your email successfully activated now you can able to login");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(FormCollection f)
        {
            string usname = f["email"].ToString();
            if (ModelState.IsValid)
            {
                var us = db.tb_Users.FirstOrDefault(m => m.Username == usname || m.Email == usname);
                if (us == null)
                {
                    return Content("false");
                }
                else
                {
                    string token = Guid.NewGuid().ToString();
                    us.ResetPasswordCode = token;
                    db.SaveChanges();
                    mail.SendEmailForgotPassword(us.Email, us.Username, token);
                    return Content("/Home");
                }
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }
        private static string MD5(string Metin)
        {
            MD5CryptoServiceProvider MD5Code = new MD5CryptoServiceProvider();
            byte[] byteDizisi = Encoding.UTF8.GetBytes(Metin);
            byteDizisi = MD5Code.ComputeHash(byteDizisi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in byteDizisi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

    }
}