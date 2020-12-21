using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaiTapTuan7.Models;
namespace BaiTapTuan7.Areas.Teacher.Model
{
    public class CourseAssignment
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public List<tb_Assignment> AssignmentList { get; set; }
    }
}