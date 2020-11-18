using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaiTapTuan7.Models
{
    public class ResetPasswordModel
    {
        [Display(Name ="New Password")]
        [Required(ErrorMessage ="Please fill password")]
        public string Password { get; set; }
        [Display(Name ="Confirm Password")]
        [Required(ErrorMessage = "Please fill password")]
        public string ConfirmPassword { get; set; }
        public string ReturnToken { get; set; }
    }
}