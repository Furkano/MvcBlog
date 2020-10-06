using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;

namespace MvcBlog.Controllers
{
    public class AdminController : Controller
    {
        mvcBlogDb db = new mvcBlogDb();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.makaleSayisi = db.Makales.Count();
            ViewBag.yorumSayisi = db.Yorums.Count();
            ViewBag.kategoriSayisi = db.Kategoris.Count();
            ViewBag.uyeSayisi = db.Uyes.Count();
            return View();
        }

    }
}