using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaiTapTuan7.Models
{
    public class ForgotPasswordModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string NewPassword { get; set; }
        public string RepeatNewPassword { get; set; }
    }
}