using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using PagedList;
using PagedList.Mvc;

namespace MvcBlog.Controllers
{
    public class HomeController : Controller
    {
        mvcBlogDb db = new mvcBlogDb();
        // GET: Home
        public ActionResult Index(int page=1)
        {
            var makale = db.Makales.OrderByDescending(m => m.Makaleid).ToPagedList(page,4);
            return View(makale);
        }

        public ActionResult BlogAra(string ara=null)
        {
            var aranan = db.Makales.Where(m => m.Baslik.Contains(ara)).ToList();
            return PartialView(aranan.OrderByDescending(t=>t.Tarih));
        }

        public ActionResult SonYorumlar()
        {
            return View(db.Yorums.OrderByDescending(y => y.Yorumid).Take(5));
        }

        public ActionResult CokOkunan()
        {
            return PartialView(db.Makales.OrderByDescending(m => m.Okunma).Take(5));
        }

        public ActionResult MakaleDetay(int id)
        {
            var makale = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        public ActionResult KategoriMakale(int id)
        {
            var makaleler = db.Makales.Where(m => m.Kategori.Kategoriid == id).ToList();
            return View(makaleler);
        }

        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
        public ActionResult KategoriPartial()
        {

            return View(db.Kategoris.ToList());
        }

        public JsonResult YorumYap(string yorum, int makaleid)
        {
            var uyeid = Session["uyeid"];
            if (yorum!=null)
            {
                db.Yorums.Add(new Yorum { UyeID = Convert.ToInt32(uyeid), MakaleID = makaleid, Icerik = yorum, Tarih = DateTime.Now });
                db.SaveChanges();
            }
            return Json(false,JsonRequestBehavior.AllowGet);
        }

        public ActionResult YorumSil(int? id)
        {
            var uyeid = Session["uyeid"];
            var yorum = db.Yorums.Where(y => y.Yorumid == id).SingleOrDefault();
            var makale = db.Makales.Where(m => m.Makaleid == yorum.MakaleID).SingleOrDefault();
            if (yorum.UyeID==Convert.ToInt32(uyeid))
            {
                db.Yorums.Remove(yorum);
                db.SaveChanges();
                return RedirectToAction("MakaleDetay", "Home", new { id = makale.Makaleid });
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult okunmaArttir(int makaleid)
        {
            var makale = db.Makales.Where(m => m.Makaleid == makaleid).SingleOrDefault();
            makale.Okunma += 1;
            db.SaveChanges();
            return View();
        }

        public ActionResult UyeProfil(int id)
        {
            var uye = db.Uyes.Where(u => u.Uyeid == id).SingleOrDefault();
            return View(uye);
        }
    }
}