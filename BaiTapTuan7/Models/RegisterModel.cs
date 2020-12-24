using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaiTapTuan7.Models
{
    public class RegisterModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }


    }
}