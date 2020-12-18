using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaiTapTuan7.Areas.Admin.Model
{
    public class AddUserModel
    {
        [Required(ErrorMessage ="This box is empty")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "This box is empty")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Usertype { get; set; }

        public string AddUserError;
    }
}