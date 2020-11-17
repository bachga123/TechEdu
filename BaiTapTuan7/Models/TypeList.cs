using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace BaiTapTuan7.Models
{
    public static class TypeList
    {
        public static List<SelectListItem> GetTypeList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            TechEduEntities db = new TechEduEntities();
            foreach(var item in db.tb_UserType)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.UserTypeId,
                    Text = item.UserTypeName
                });
            }
            return list;
        }
    }
}