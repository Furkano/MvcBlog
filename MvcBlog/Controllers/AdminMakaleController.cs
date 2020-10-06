using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace MvcBlog.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcBlogDb db = new mvcBlogDb();
        // GET: AdminMakale
        public ActionResult Index(int Page=1)
        {
            var makales = db.Makales.OrderByDescending(m=>m.Makaleid).ToPagedList(Page,5);
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.Kategoriid = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale, string etiketler, HttpPostedFileBase Foto)
        {

            // TODO: Add insert logic here
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newFoto);
                    makale.Foto = "/Uploads/MakaleFoto/" + newFoto;
                }
                if (etiketler != null)
                {
                    string[] etiketDizi = etiketler.Split(',');
                    foreach (var item in etiketDizi)
                    {
                        var yeniEtiket = new Etiket { EtiketAdi = item };
                        db.Etikets.Add(yeniEtiket);
                        makale.Etikets.Add(yeniEtiket);
                    }
                }
                makale.Uyeid = Convert.ToInt32(Session["uyeid"]);
                makale.Okunma = 0;
                db.Makales.Add(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(makale);

        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound ();
            }
            ViewBag.KategoriId = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi", makale.KategoriID);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Makale makale)
        {
            try
            {
                ViewBag.KategoriId = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi", makale.KategoriID);
                // TODO: Add update logic here
                var makales=db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
                if (Foto!=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newFoto);
                    makales.Foto = "/Uploads/MakaleFoto/" + newFoto;
                    makales.Baslik = makale.Baslik;
                    makales.Icerik = makale.Icerik;
                    makales.KategoriID = makale.KategoriID;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(makale);
            }
            catch
            {
                ViewBag.KategoriId = new SelectList(db.Kategoris, "Kategoriid", "KategoriAdi", makale.KategoriID);
                return View(makale);
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var makales = db.Makales.Where(m => m.Makaleid == id).SingleOrDefault();
                if (makales==null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Foto));
                }
                foreach (var item in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(item);
                }
                foreach (var item in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(item);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
