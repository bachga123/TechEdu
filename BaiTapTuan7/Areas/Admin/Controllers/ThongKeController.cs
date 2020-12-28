using BaiTapTuan7.Areas.Admin.Model;
using BaiTapTuan7.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Security.Cryptography;
using System.Text;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class ThongKeController : Controller
    {
        TechEduEntities db = new TechEduEntities();

        public ActionResult ThongKeUserType()
        {
            var admin = (tb_Users)Session["user"];
            var list = db.tb_Users.ToList();
            var item = list.FirstOrDefault(m => m.Id == admin.Id);
            var model = new List<ThongKeViewModel>();
            if (item != null)
            {
                list.Remove(item);

                var tb_UserType = db.tb_UserType.ToList();
                var tb_Users = db.tb_Users.ToList();
                foreach (var usertype in tb_UserType)
                {
                    var thongKeViewModel = new ThongKeViewModel();
                    thongKeViewModel.UserType = usertype.UserTypeName;
                    thongKeViewModel.UserCount = tb_Users.Where(x => x.Usertype == usertype.UserTypeId).Count();
                    model.Add(thongKeViewModel);
                }
            }
            return View(model);
        }

    }
}