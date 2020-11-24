using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaiTapTuan7.Models;
using PagedList;
namespace BaiTapTuan7.Areas.Admin.Model
{
    public class PagedData
    {
        TechEduEntities db = new TechEduEntities();
        public IEnumerable<tb_Users> ListAllPagingUser(int page, int pageSize)
        {
            return db.tb_Users.ToPagedList(page, pageSize);
        }
    }
}