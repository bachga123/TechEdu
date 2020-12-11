using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaiTapTuan7.Areas.Teacher.Model
{
    public class AssignmentScore
    { 
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Answer { get; set; }
        public float Score { get; set; }
    }
}