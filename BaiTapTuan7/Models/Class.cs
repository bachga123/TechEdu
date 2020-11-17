using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaiTapTuan7.Models
{
    public class Class
    {
        [Display(Name = "Id")]
        public int ClassId { get; set; }
        [Display(Name = "Name")]
        public string ClassName { get; set; }
        [Display(Name = "Year")]
        public Nullable<int> YearOfClass { get; set; }
    }
}