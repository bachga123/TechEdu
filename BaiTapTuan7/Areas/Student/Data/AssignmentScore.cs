using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaiTapTuan7.Areas.Student.Data
{
    public class AssignmentScore
    {
        public string AssigmentDetails { get; set; }
        public string File { get; set; }
        public float Score { get; set; }
        
        public string CommentOfTeacher { get; set; }
    }
}