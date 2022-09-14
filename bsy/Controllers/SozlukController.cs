using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using bsy.Filters;
using bsy.Helpers;
using bsy.Models;

namespace bsy.Controllers
{
    //[OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class SozlukController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Sozluk
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListeRol(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from s in context.tblSozluk
                         where s.Turu == SozlukHelper.rolKodu
                         select s);

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("id").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from s in resultSetAfterOrderandPaging
                             select new
                             {
                                 s.id,
                                 s.Turu,
                                 rol=s.Aciklama,
                                 Degistir = 0,
                                 Sil = 0
                             }).ToList();


            //int totalRecords = resultSet.Count();
            //int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            //Data to sent grid
            var jsonData = new
            {
                total = totalPages, //todo: calculate
                page = page,
                records = totalRecords,
                rows = (
                  from s in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 s.id.ToString(),
                                 s.Turu,
                                 s.rol,
                                 s.Degistir.ToString(),
                                 s.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rol()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult YeniRol(long id=0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK soz = null;
            try
            {
                soz = context.tblSozluk.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (soz == null)
            {
                soz = new SOZLUK();
                soz.Turu = SozlukHelper.rolKodu;
            }

            return View(soz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniRol(SOZLUK yeniSozluk, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniSozluk);
            }

            SOZLUK eskiSozluk = context.tblSozluk.Find(yeniSozluk.id);
            if (eskiSozluk == null)
            {
                eskiSozluk = new SOZLUK();
            }

            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            eskiSozluk = SozlukYeniToEski(eskiSozluk, yeniSozluk);

            if (eskiSozluk.id == 0)
            {
                SOZLUK soz = context.tblSozluk.Where(sz => sz.Turu == SozlukHelper.rolKodu && sz.Aciklama == eskiSozluk.Aciklama).FirstOrDefault();
                if (soz != null)
                {
                    m = new Mesaj("hata", "Bu rol kaydı daha önce oluşturulmuş, tekrar eklenemez");
                    mesajlar.Add(m);
                    Session["MESAJLAR"] = mesajlar;
                    return View(yeniSozluk);
                }
                else
                {
                    context.tblSozluk.Add(eskiSozluk);
                    m = new Mesaj("tamam", "Rol Kaydı Eklenmiştir.");
                }
            }
            else
            {
                context.Entry(eskiSozluk).State = EntityState.Modified;
                m = new Mesaj("tamam", "Rol Kaydı Güncellenmiştir.");
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Rol kaydı güncelleneMEdi");
            }


            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Rol", false);
            return Content("OK");

        }

        private SOZLUK SozlukYeniToEski(SOZLUK eskiSozluk, SOZLUK yeniSozluk)
        {
            eskiSozluk.id = yeniSozluk.id;
            eskiSozluk.Turu = yeniSozluk.Turu;
            eskiSozluk.Aciklama = yeniSozluk.Aciklama;

            return eskiSozluk;
        }
        public ActionResult RolSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            SOZLUK sozluk = context.tblSozluk.Find(id);
            context.Entry(sozluk).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Rol Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Rol Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Sozluk/Rol", false);
            return Content("OK");

        }
    }
}