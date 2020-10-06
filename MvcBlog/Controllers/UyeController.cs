using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;

namespace MvcBlog.Controllers
{
    public class UyeController : Controller
    {
        mvcBlogDb mvc = new mvcBlogDb();

        public object Cyrpto { get; private set; }

        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = mvc.Uyes.Where(u => u.Uyeid == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"])!=uye.Uyeid)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Uye uye)
        {
            
            var login = mvc.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
            if (login.KullaniciAdi==uye.KullaniciAdi && login.Sifre==uye.Sifre && login.Mail==uye.Mail)
            {
                Session["uyeid"] = login.Uyeid;
                Session["kullaniciadi"] = login.KullaniciAdi;
                Session["yetkiid"] = login.YetkiID;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Uyari = "Kulanıcı adı , maili veya şifrenizi kontrol ediniz.";
                return View();
            }
            
        }
        public ActionResult Logout()
        {
            Session["uyeid"] = null;
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye,HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto!=null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newFoto);
                    uye.Foto = "/Uploads/UyeFoto/" + newFoto;
                    uye.YetkiID = 2;
                    mvc.Uyes.Add(uye);
                    mvc.SaveChanges();
                    Session["uyeid"] = uye.Uyeid;
                    Session["kullaniciadi"] = uye.KullaniciAdi;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("fotograf", "Fotograf seçiniz");

                }
            }
            return View(uye);
        }
        public ActionResult Edit(int id)
        {
            var uye = mvc.Uyes.Where(u => u.Uyeid == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.Uyeid)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        [HttpPost]
        public ActionResult Edit(Uye uye,int id,HttpPostedFileBase Foto)
        {
            
            if (ModelState.IsValid)
            {
                var editUye = mvc.Uyes.Where(u => u.Uyeid == uye.Uyeid).SingleOrDefault();
                if (Foto!=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(editUye.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(editUye.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newFoto);
                    editUye.Foto = "/Uploads/UyeFoto/" + newFoto;
                }
                editUye.AdSoyad = uye.AdSoyad;
                editUye.KullaniciAdi = uye.KullaniciAdi;
                editUye.Mail = uye.Mail;
                editUye.Sifre = uye.Sifre;
                mvc.SaveChanges();
                Session["kullaniciadi"] = uye.KullaniciAdi;
                return RedirectToAction("Index", "Home", new { id = editUye.Uyeid });
            }
            return View();
        }
    }
}