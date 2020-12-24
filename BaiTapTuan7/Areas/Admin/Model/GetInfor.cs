using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaiTapTuan7.Models;

namespace BaiTapTuan7.Areas.Admin.Model
{
    public class GetInfor
    {
        TechEduEntities db = new TechEduEntities();
        public tb_Student GetStudent(int studentid)
        {
            return db.tb_Student.Find(studentid);
        }
        public tb_Course GetCourse(int couid)
        {
            return db.tb_Course.Find(couid);
        }
        public tb_Teacher GetTeacher(int tcid)
        {
            return db.tb_Teacher.Find(tcid);
        }
    }
}