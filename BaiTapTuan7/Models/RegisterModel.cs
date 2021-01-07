using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaiTapTuan7.Models
{
    public class RegisterModel
    {

        public string FirstName { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        public string Gmail { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }


    }
}