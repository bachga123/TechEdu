//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaiTapTuan7.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tb_Course
    {
        public int Course_Id { get; set; }
        [Required(ErrorMessage = "Course must have name")]
        [Display(Name = "Course name")]
        [MaxLength(150, ErrorMessage = "You can have a max of 150 characters")]
        public string Course_Name { get; set; }
        [Required(ErrorMessage = "Course must have Description")]
        [Display(Name = "Course Decription")]
        [MaxLength(300, ErrorMessage = "You can have a max of 300 characters")]
        public string Decription { get; set; }
        [Display(Name = "Course details")]
        [MaxLength(500, ErrorMessage = "You can have a max of 500 characters")]
        public string Details { get; set; }
        [Required(ErrorMessage = "Course must have teacher")]
        public Nullable<int> TeacherId { get; set; }
        [Display(Name = "Course price")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> Course_Price { get; set; }
    }
}
