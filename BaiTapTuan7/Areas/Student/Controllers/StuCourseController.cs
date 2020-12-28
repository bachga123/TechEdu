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
using BaiTapTuan7.Areas.Student.Data;
using PayPal.Api;
using System.Globalization;

namespace BaiTapTuan7.Areas.Student.Controllers
{
    [AuthorizeController]
    public class StuCourseController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Student/Course
        public ActionResult Index()
        {
            var couList = MyCourseList();
            foreach (var item in couList)
            {
                CheckIfOutDateForPayment(item.Course_Id);
            }
            return View(MyCourseList());
        }
        public ActionResult AllOfMyCourse(int page = 1, int pageSize = 10)
        {
            tb_Student stu = (tb_Student)Session["student"];
            var courseList = db.tb_Course.ToList();
            var listOfMyCourse = db.tb_StudentCourse.Where(m => m.StudentId == stu.StudentId);
            List<tb_Course> cls = new List<tb_Course>();
            foreach (var item in listOfMyCourse)
            {
                if (item.EnrollDate == null && item.Status == null && item.Payment == null)
                {
                }
                else
                {
                    var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.CourseId);
                    cls.Add(cou);
                }
            }
            foreach (var item in cls)
            {
                var cou = db.tb_Course.Find(item.Course_Id);
                courseList.Remove(cou);
            }
            var pageList = courseList.OrderByDescending(m => m.Course_Name).ToPagedList(page, pageSize);
            return View("AllOfMyCourse", pageList);
        }
        public ActionResult EnrollToCourse(int? couid)
        {
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                var cou = db.tb_Course.Find(couid);
                tb_Student stu = (tb_Student)Session["student"];
                tb_StudentCourse cts1 = db.tb_StudentCourse.FirstOrDefault(m => m.CourseId == couid && m.StudentId == stu.StudentId);
                if(cts1 != null)
                {
                    Session["course"] = cou;
                    return RedirectToAction("PaymentWithPaypal");
                }
                else
                {
                    cts1 = new tb_StudentCourse();
                }
                cts1.CourseId = couid;
                cts1.StudentId = stu.StudentId;
                cts1.EnrollDate = DateTime.Now;
                if (cou.Course_Price != null)
                {
                    cts1.Payment = false;
                    //return RedirectToAction("PaymentForCourse", new { couid });
                }
                else
                {
                    cts1.Payment = true;
                }
                cts1.Status = 1;
                db.Entry(cts1).State = EntityState.Added;
                db.SaveChanges();
            }
            return RedirectToAction("AllOfMyCourse");
        }
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/StuCourse/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //on successful payment, show success page to user.
            InCourse(((tb_Course)Session["course"]).Course_Id);
            return RedirectToAction("CourseDetails", "StuCourse",new { couid = ((tb_Course)Session["course"]).Course_Id});
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var courseDetails = (tb_Course)Session["course"];
            string priceTemp = ((Decimal)courseDetails.Course_Price).ToString("F", new CultureInfo("en-US"));
                 //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = courseDetails.Course_Name,
                currency = "USD",
                price = priceTemp,
                quantity = "1",
                sku = "Course"
            }) ;
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = priceTemp
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = priceTemp, // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Payment for "+ courseDetails.Course_Name,
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
        public ActionResult OutCourse(int? couid)
        {
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                tb_Student stu = (tb_Student)Session["student"];
                tb_StudentCourse cts = db.tb_StudentCourse.FirstOrDefault(m => m.StudentId == stu.StudentId && m.CourseId == couid);
                cts.Status = null;
                cts.EnrollDate = null;
                cts.Payment = null;
                db.Entry(cts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult InCourse(int? couid)
        {
            if (couid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                tb_Student stu = (tb_Student)Session["student"];
                tb_StudentCourse cts = db.tb_StudentCourse.FirstOrDefault(m => m.StudentId == stu.StudentId && m.CourseId == couid);
                cts.Status = 1;
                cts.EnrollDate = DateTime.Now;
                cts.Payment = true;
                db.Entry(cts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult CourseDetails(int? couid)
        {
            tb_Course cou = db.tb_Course.Find(couid);
            var stu = (tb_Student)Session["student"];
            ViewBag.Teacher = db.tb_Teacher.Find(cou.TeacherId);
            ViewBag.contentList = MyContentList(cou.Course_Id);
            ViewBag.couid = cou.Course_Id;
            var check = db.tb_StudentCourse.FirstOrDefault(m => m.CourseId == couid && m.StudentId == stu.StudentId);
            if (check.Payment == false && check.Status == 1)
            {
                TimeSpan tp = ((DateTime)check.EnrollDate).AddDays(7) - DateTime.Now;
                ViewBag.messageOutDate = tp.Days;
            }
            Session["courseid"] = cou.Course_Id;
            return View("CourseDetails", cou);
        }

        // Assignment
        public ActionResult CourseAssignment(int couid)
        {
            ViewBag.couid = couid;
            List<tb_Assignment> assList = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            return View("CourseAssignment", assList);
        }
        public ActionResult AssignmentDetails(int assid)
        {
            var studentAnswerList = db.tb_Student_Assignment.Where(m => m.Assignment_Id == assid);
            ViewBag.studentAnswerList = studentAnswerList;
            tb_Assignment ass = db.tb_Assignment.Find(assid);
            ViewBag.couid = ass.Course_Id;
            return View("AssignmentDetails", ass);
        }

        public ActionResult AnswerAssignment(int assid)
        {
            tb_Student stu = (tb_Student)Session["student"];
            var tsa = db.tb_Student_Assignment.FirstOrDefault(m => m.Assignment_Id == assid && m.Student_Id == stu.StudentId);
            if (tsa != null)
            {
                return RedirectToAction("EditAnswerAssignment", new { asid = tsa.Assignment_Id });
            }
            else
            {
                tsa = new tb_Student_Assignment();
                tsa.Assignment_Id = assid;
                tsa.Student_Id = stu.StudentId;
                return View("AnswerAssignment", tsa);
            }
        }
        [HttpPost]
        public ActionResult AnswerAssignment(tb_Student_Assignment tsa, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                tb_Student_Assignment a = new tb_Student_Assignment();
                a.Assignment_Id = tsa.Assignment_Id;
                a.Student_Id = tsa.Student_Id;
                a.Assignment_Id = tsa.Assignment_Id;
                if (file != null)
                {
                    a.File = this.AddFileToFiles_Here(file);
                }
                a.Decriptions = tsa.Decriptions;
                db.Entry(a).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails", new { assid = tsa.Assignment_Id });
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
        public ActionResult EditAnswerAssignment(int? asid)
        {
            tb_Student stu = (tb_Student)Session["student"];
            var tsa = db.tb_Student_Assignment.FirstOrDefault(m => m.Assignment_Id == asid && m.Student_Id == stu.StudentId);
            if (tsa == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View("EditAnswerAssignment", tsa);
        }
        [HttpPost]
        public ActionResult EditAnswerAssignment(tb_Student_Assignment tsa, HttpPostedFileBase file)
        {
            var result = db.tb_Student_Assignment.FirstOrDefault(m => m.Assignment_Id == tsa.Assignment_Id && m.Student_Id == tsa.Student_Id);
            if (result != null && ModelState.IsValid)
            {
                if (file != null)
                {
                    result.File = this.AddFileToFiles_Here(file);
                }
                result.Decriptions = tsa.Decriptions;
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails", new { assid = tsa.Assignment_Id });
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
                var aas = db.tb_Student_Assignment.Find(assid);
                aas.File = null;
                //cần delte file trong Files_Here
                db.Entry(aas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EditAnswerAssignment", new { asid = aas.Assignment_Id });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        //Xem diem cua assignment minh da lam
        public ActionResult MyAssignmentDid(int page = 1, int pageSize = 10)
        {
            ViewBag.couid = (int)Session["courseid"];
            return View("MyAssignmentDid", MyAnswerAssignmentList().ToPagedList(page, pageSize));
        }
        public List<tb_Course> MyCourseList()
        {
            tb_Student stu = (tb_Student)Session["student"];
            var data = db.tb_StudentCourse.Where(m => m.StudentId == stu.StudentId && m.Status == 1).ToList();
            List<tb_Course> myCourseList = new List<tb_Course>();
            foreach (var item in data)
            {
                var cou = db.tb_Course.FirstOrDefault(m => m.Course_Id == item.CourseId);
                myCourseList.Add(cou);
            }
            return myCourseList;
        }
        public List<AssignmentScore> MyAnswerAssignmentList()
        {
            int couid = (int)Session["courseid"];
            List<AssignmentScore> assList = new List<AssignmentScore>();
            tb_Student stu = (tb_Student)Session["student"];
            var assOfCourseList = db.tb_Assignment.Where(m => m.Course_Id == couid).ToList();
            foreach (var item in assOfCourseList)
            {
                AssignmentScore ass = new AssignmentScore();
                var result = db.tb_Student_Assignment.Where(m => m.Student_Id == stu.StudentId && m.Assignment_Id == item.Assignment_Id);
                if (result != null)
                {
                    ass.AssigmentDetails = item.Details;
                    ass.File = item.File;
                    var sco = db.tb_Score.FirstOrDefault(m => m.Student_id == stu.StudentId && m.Assignment_id == item.Assignment_Id);
                    if (sco != null)
                    {
                        ass.Score = (float)sco.Score;
                        ass.CommentOfTeacher = sco.details;
                    }
                    else
                    {
                        ass.Score = -1;
                        ass.CommentOfTeacher = "Teacher not comment yet !";
                    }
                    if (assList.Contains(ass) == false)
                    {
                        assList.Add(ass);
                    }
                }
            }
            return assList;
        }

        public List<tb_Content> MyContentList(int couid)
        {
            List<tb_Course_Content> tcc = db.tb_Course_Content.Where(m => m.Course_Id == couid).ToList();
            List<tb_Content> tc = new List<tb_Content>();
            foreach (var item in tcc)
            {
                tb_Content tc1 = db.tb_Content.Find(item.Content_Id);
                if (tc.Contains(tc1) == false)
                {
                    tc.Add(tc1);
                }
            }
            return tc;
        }

        public bool CheckIfOutDateForPayment(int couid)
        {
            var stu = (tb_Student)Session["student"];
            var check = db.tb_StudentCourse.FirstOrDefault(m => m.StudentId == stu.StudentId && m.CourseId == couid);
            if (check == null)
                return false;
            if (check.Status == 1 && check.Payment == false)
            {
                TimeSpan tp = ((DateTime)check.EnrollDate).AddDays(7) - DateTime.Now;
                if (tp < TimeSpan.Zero)
                {
                    OutCourse(couid);
                    return false;
                }
            }
            return true;

        }
    }
}