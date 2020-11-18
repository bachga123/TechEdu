using BaiTapTuan7.Models;
using BaiTapTuan7.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaiTapTuan7.Controllers
{
    [AuthorizeController]
    public class ClassController : Controller
    {
        // GET: Class
        public ActionResult Index()
        {
            ClassClient cc = new ClassClient();
            ViewBag.listClass = cc.findAll();
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ClassViewModel cvm)
        {
            ClassClient cc = new ClassClient();
            cc.Create(cvm.clas);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            ClassClient cc = new ClassClient();
            cc.Delete(id);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ClassClient cc = new ClassClient();
            ClassViewModel cvm = new ClassViewModel();
            cvm.clas = cc.find(id);
            return View("Edit", cvm);
        }
        [HttpPost]
        public ActionResult Edit(ClassViewModel cvm)
        {
            ClassClient cc = new ClassClient();
            cc.Edit(cvm.clas);
            return RedirectToAction("Index");
        }
    }
}