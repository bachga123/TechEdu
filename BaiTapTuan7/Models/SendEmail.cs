using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using BaiTapTuan7.Resources.Constants;
using System.Web.Mvc;
namespace BaiTapTuan7.Models
{
    public class SendEmail
    {
        HttpRequest Request = HttpContext.Current.Request;
        public bool SendEmailForgotPassword(string ToEmail,string username,string ForgotPasswordCode)
        {
            string emailSubject = EmailInfor.EMAIL_SUBJECT_DEFAUALT + " Test";
            string body = "Hello " + username + ", ";
            body += "<br /><br />Please click the following link to reset your password account";
            body += "<br /><a href = '" + string.Format("{0}://{1}/Home/ActiveAccount/{2}", Request.Url.Scheme, Request.Url.Authority, ForgotPasswordCode) + "'>Click here to reset your password account.</a>";
            body += "<br /><br />Thanks";
            try
            {
                SendEmailAsync(ToEmail, body, emailSubject);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ActiveAccount(string ToEmail,string message)
        {
            string body = "Hello " + ToEmail + ",";
            string emailSubject = EmailInfor.EMAIL_SUBJECT_DEFAUALT + " Test";
            try
            {
                SendEmailAsync(ToEmail, message, emailSubject);
                return true;
            }
            catch
            {
                return false;

            }

        }
        public bool SendVerificationLinkMail(string ToEmail,string activationCode)
        {

            string body = "Hello " + ToEmail + ",";
            string emailSubject = EmailInfor.EMAIL_SUBJECT_DEFAUALT + " Test";
            body += "<br /><br />Please click the following link to activate your account";
            body += "<br /><a href = '" + string.Format("{0}://{1}/Home/ActiveAccount/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
            body += "<br /><br />Thanks";
            try
            {
                SendEmailAsync(ToEmail, body, emailSubject);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool EnrollStudentSuccessMail(string ToEmail,string CourseName)
        {
            // Initialization.  
            string emailMsg = "Dear " + ToEmail + ", <br /><br /> You have been enroll to " + CourseName + " <b style='color: red'> Notification </b> <br /><br /> Thanks & Regards, <br />Asma Khalid";
            string emailSubject = EmailInfor.EMAIL_SUBJECT_DEFAUALT + " Test";
            // Sending Email.
            try
            {
                SendEmailAsync(ToEmail, emailMsg, emailSubject);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SendEmailAsync(string email, string msg, string subject = "")
        {
            // Initialization.
            var isSend = false;

            try
            {
                // Initialization.  
                var body = msg;
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress(EmailInfor.FROM_EMAIL_ACCOUNT, "Tech Edu University");
                message.Subject = !string.IsNullOrEmpty(subject) ? subject : EmailInfor.EMAIL_SUBJECT_DEFAUALT;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    // Settings.  
                    var credential = new NetworkCredential
                    {
                        UserName = EmailInfor.FROM_EMAIL_ACCOUNT,
                        Password = EmailInfor.FROM_EMAIL_PASSWORD
                    };

                    // Settings.  
                    smtp.Credentials = credential;
                    smtp.Host = EmailInfor.SMTP_HOST_GMAIL;
                    smtp.Port = Convert.ToInt32(EmailInfor.SMTP_PORT_GMAIL);
                    smtp.EnableSsl = true;

                    // Sending  
                    smtp.Send(message);

                    isSend = true;
                }
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
            return isSend;
        }
    }
}