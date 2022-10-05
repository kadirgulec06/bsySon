using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.KisiHane;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
    public class KisiHaneController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: KisiHane
        public ActionResult ListeKisiler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, int haneliHanesiz = 1)
        {
            User user = (User)Session["USER"];

            string adSoyad = "";
            string tcNo = "";

            if (Request.Params["_search"] == "true")
            {

                if (Request.Params["ADSOYAD"] != null)
                {
                    adSoyad = Request.Params["ADSOYAD"];
                }

                if (Request.Params["TCNO"] != null)
                {
                    tcNo = Request.Params["TCNO"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreADSOYAD"] = adSoyad.ToUpper();
                Session["filtreTCNO"] = tcNo.ToUpper();
            }
            else if (rapor == 1)
            {
                adSoyad = (string)Session["filtreADSOYAD"];
                tcNo = (string)Session["filtreTCNO"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            /*
            DateTime bugun = DateTime.Now.Date;


            var sonKisiHaneTarih = (from kh in context.tblKisiHane
                               join hn in context.tblHaneler on kh.HaneID equals hn.id
                               join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                               where
                                 kh.BitTar > bugun &&
                                 (kh.HaneID == haneID || haneID == 0) &&
                                 (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                               group kh by kh.KisiID into khGRP
                               select new { KisiID=khGRP.Key, Tarih = khGRP.Max(x => x.BasTar) });

            var sonKisiHane = (from kht in sonKisiHaneTarih
                               join kh in context.tblKisiHane on new { x1 = kht.KisiID, x2 = kht.Tarih } equals new { x1 = kh.KisiID, x2 = kh.BasTar }
                               select kh);
            */

            var tumKisiler = from kx in context.tblKisiler
                             select kx.id;

            if (haneliHanesiz == 2)
            {
                DateTime bugun = DateTime.Now.Date;

                var haneKisileri = from kx in context.tblKisiler
                                   join kh in context.tblKisiHane on kx.id equals kh.KisiID
                                   where kh.BitTar > bugun
                                   select kx.id;

                tumKisiler = tumKisiler.Except(haneKisileri).Distinct();
            }

            var kisiler = (from hk in tumKisiler
                         join kx in context.tblKisiler on hk equals kx.id
                         select new
                         {
                             kx.id,
                             AdSoyad = kx.Ad + " " + kx.Soyad,
                             kx.TCNo
                         });

            int totalRecords = kisiler.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = kisiler.OrderBy("AdSoyad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kx in resultSetAfterOrderandPaging
                             select new
                             {
                                 kx.id,
                                 kx.AdSoyad,
                                 kx.TCNo,
                                 Ekle=0,
                                 Haneler = 0
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
                  from kx in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 kx.id.ToString(),
                                 kx.AdSoyad,
                                 kx.TCNo,
                                 kx.Ekle.ToString(),
                                 kx.Haneler.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int haneliHanesiz = 1)
        {
            ViewBag.ilkGiris = 1;
            ViewBag.haneliHanesiz = haneliHanesiz;

            return View();
        }

        public ActionResult ListeHaneler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long mahalleID = 0)
        {
            User user = (User)Session["USER"];
            string sehirIlce = "";
            string mahalle = "";
            string adres = "";
            string haneKodu = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["SEHIRILCE"] != null)
                {
                    sehirIlce = Request.Params["SEHIRILCE"];
                }

                if (Request.Params["MAHALLE"] != null)
                {
                    mahalle = Request.Params["MAHALLE"];
                }

                if (Request.Params["ADRES"] != null)
                {
                    adres = Request.Params["ADRES"];
                }

                if (Request.Params["HANEKODU"] != null)
                {
                    haneKodu = Request.Params["HANEKODU"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIRILCE"] = sehirIlce.ToUpper();
                Session["filtreMAHALLE"] = mahalle.ToUpper();
                Session["filtreADRES"] = adres.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
            }
            else if (rapor == 1)
            {
                sehirIlce = (string)Session["filtreSEHIRILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
                adres = (string)Session["filtreADRES"];
                haneKodu = (string)Session["filtreHANEKODU"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from h in context.tblHaneler
                         join mh in context.tblMahalleler on h.MahalleID equals mh.id
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (h.MahalleID == mahalleID || mahalleID == 0) &&
                            (sz.Aciklama + " " + sy.Aciklama + "").Contains(sehirIlce) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (h.Cadde + " " + h.Sokak + " " + h.Apartman + " " + h.Daire + "").Contains(adres) &&
                            (h.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             h.id,
                             h.HaneKodu,
                             sehirIlce = sz.Aciklama + "-" + sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             Adres = h.Cadde + " " + h.Sokak + " " + h.Apartman + "-" + h.Daire
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("HaneKodu").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from h in resultSetAfterOrderandPaging
                             select new
                             {
                                 h.id,
                                 h.HaneKodu,
                                 h.sehirIlce,
                                 h.mahalleADI,
                                 h.Adres,
                                 Sec = 0
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
                  from h in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 h.id.ToString(),
                                 h.HaneKodu,
                                 h.sehirIlce,
                                 h.mahalleADI,
                                 h.Adres,
                                 h.Sec.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Mahalle(long kisiID)
        {
            Session["kisiID"] = kisiID;

            ViewBag.ilkGiris = 1;
            ViewBag.mahalleID = 0;

            return View();
        }

        public ActionResult HaneSecildi(long haneID)
        {
            Session["haneID"] = haneID;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/KisiHane/YeniKisiHane?id=0", false);

            return Content("OK");

        }


        public ActionResult ListeKisiHane(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long kisiID=0)
        {
            User user = (User)Session["USER"];

            string sehirIlce = "";
            string mahalle = "";
            string haneKodu = "";
            string adres = "";
            string adSoyad = "";
            string tcNo = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["SEHIRILCE"] != null)
                {
                    sehirIlce = Request.Params["SEHIRILCE"];
                }

                if (Request.Params["MAHALLE"] != null)
                {
                    mahalle = Request.Params["MAHALLE"];
                }

                if (Request.Params["HANEKODU"] != null)
                {
                    haneKodu = Request.Params["HANEKODU"];
                }

                if (Request.Params["ADRES"] != null)
                {
                    adres = Request.Params["ADRES"];
                }

                if (Request.Params["ADSOYAD"] != null)
                {
                    adSoyad = Request.Params["ADSOYAD"];
                }

                if (Request.Params["TCNO"] != null)
                {
                    tcNo = Request.Params["TCNO"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIRILCE"] = sehirIlce.ToUpper();
                Session["filtreMAHALLE"] = mahalle.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
                Session["filtreADRES"] = adres.ToUpper();
                Session["filtreADSOYAD"] = adSoyad.ToUpper();
                Session["filtreTCNO"] = tcNo.ToUpper();
            }
            else if (rapor == 1)
            {
                sehirIlce = (string)Session["filtreSEHIRILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
                haneKodu = (string)Session["filtreHANEKODU"];
                adres = (string)Session["filtreADRES"];
                adSoyad = (string)Session["filtreADSOYAD"];
                tcNo = (string)Session["filtreTCNO"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            /*
            DateTime bugun = DateTime.Now.Date;


            var sonKisiHaneTarih = (from kh in context.tblKisiHane
                               join hn in context.tblHaneler on kh.HaneID equals hn.id
                               join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                               where
                                 kh.BitTar > bugun &&
                                 (kh.HaneID == haneID || haneID == 0) &&
                                 (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                               group kh by kh.KisiID into khGRP
                               select new { KisiID=khGRP.Key, Tarih = khGRP.Max(x => x.BasTar) });

            var sonKisiHane = (from kht in sonKisiHaneTarih
                               join kh in context.tblKisiHane on new { x1 = kht.KisiID, x2 = kht.Tarih } equals new { x1 = kh.KisiID, x2 = kh.BasTar }
                               select kh);
            */

            var query = (from kx in context.tblKisiler
                         join kh in context.tblKisiHane on kx.id equals kh.KisiID
                         join hn in context.tblHaneler on kh.HaneID equals hn.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sm in context.tblSozluk on mh.id equals sm.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (kx.id == kisiID || kisiID == 0) &&
                            (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id)) &&
                            (sm.Aciklama + "").Contains(mahalle) &&
                            (sz.Aciklama + " " + sy.Aciklama + "").Contains(sehirIlce) &&
                            (hn.HaneKodu + "").Contains(haneKodu) &&
                            (hn.Cadde + "" + hn.Sokak + " " + hn.Apartman + "-" + hn.Daire + "").Contains(adres) &&
                            (kx.Ad + " " + kx.Soyad + "").Contains(adSoyad) &&
                            (kx.TCNo + "").Contains(tcNo)
                         select new
                         {
                             kh.id,
                             hn.HaneKodu,
                             sehirIlce = sz.Aciklama + " " + sy.Aciklama,
                             mahalleADI = sm.Aciklama,
                             Adres = hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + "-" + hn.Daire,
                             AdSoyad = kx.Ad + " " + kx.Soyad,
                             kx.TCNo,
                             kh.BasTar,
                             kh.BitTar
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("AdSoyad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kx in resultSetAfterOrderandPaging
                             select new
                             {
                                 kx.id,
                                 kx.HaneKodu,
                                 kx.sehirIlce,
                                 kx.mahalleADI,
                                 kx.Adres,
                                 kx.AdSoyad,
                                 kx.TCNo,
                                 kx.BasTar,
                                 kx.BitTar,
                                 Degistir = 0
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
                  from kx in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 kx.id.ToString(),
                                 kx.HaneKodu,
                                 kx.sehirIlce,
                                 kx.mahalleADI,
                                 kx.Adres,
                                 kx.AdSoyad,
                                 kx.TCNo,
                                 kx.BasTar.ToShortDateString(),
                                 kx.BitTar.ToShortDateString(),
                                 kx.Degistir.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KisiHane(long kisiID)
        {
            Session["kisiID"] = kisiID;
            ViewBag.kisiID = kisiID;

            return View();
        }

        public ActionResult YeniKisiHane(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m;

            KisiHaneVM khVM = new KisiHaneVM();
            khVM.id = id;

            long kisiID = 0;
            long haneID = 0;
            if (id == 0)
            {
                try
                {
                    kisiID = (long)Session["kisiID"];
                    haneID = (long)Session["haneID"];
                }
                catch { }

                khVM.BasTar = DateTime.Now.Date;
                khVM.BitTar = new DateTime(3000, 1, 1);
            }
            else
            {
                KISIHANE kh = context.tblKisiHane.Find(id);

                kisiID = kh.KisiID;
                haneID = kh.KisiID;
                khVM.BasTar = kh.BasTar;
                khVM.BitTar = kh.BitTar;
            }

            khVM.kunyeKisi = SozlukHelper.KunyeHazirla(context, kisiID);
            khVM.kunyeHane = SozlukHelper.KunyeHazirla(context, haneID);

            if (kisiID == 0 || haneID == 0)
            {
                m = new Mesaj("hata", "Kişi ve Hane seçiniz");
                mesajlar.Add(m);
                Session["MESAJLAR"] = mesajlar;

                return View(khVM);
            }

            Session["kisiID"] = kisiID;
            Session["haneID"] = haneID;

            ViewBag.ilkGiris = 0;
            ViewBag.mahalleID = 0;

            return View(khVM);
        }

        [HttpPost]
        public ActionResult YeniKisiHane(KisiHaneVM khVM)
        {

            khVM.kunyeKisi = SozlukHelper.KunyeHazirla(context, khVM.kunyeKisi.kunyeID.KisiID);
            khVM.kunyeHane = SozlukHelper.KunyeHazirla(context, khVM.kunyeHane.kunyeID.HaneID);

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(khVM);
            }

            DateTime bugun = DateTime.Now.Date;
            KISIHANE kisiHane = new KISIHANE();

            if (khVM.id == 0)
            {
                var kh = (from khx in context.tblKisiHane
                          where khx.KisiID == khVM.kunyeKisi.kunyeID.KisiID &&
                                khx.HaneID == khVM.kunyeHane.kunyeID.HaneID &&
                                khx.BitTar > bugun
                          select khx).FirstOrDefault();

                if (kh != null)
                {
                    m = new Mesaj("hata", "Kişi bu haneye kayıtlı");
                    mesajlar.Add(m);
                    Session["MESAJLAR"] = mesajlar;
                    return View(khVM);
                }

                kh = (from khx in context.tblKisiHane
                          where khx.KisiID == khVM.kunyeKisi.kunyeID.KisiID &&
                                khx.HaneID != khVM.kunyeHane.kunyeID.HaneID &&
                                khx.BitTar > bugun
                          select khx).FirstOrDefault();

                if (kh != null)
                {
                    m = new Mesaj("hata", "Kişi başka bir haneye kayıtlı");
                    mesajlar.Add(m);
                    Session["MESAJLAR"] = mesajlar;
                    return View(khVM);
                }

                //kisiHane.KisiID = khVM.kunyeKisi.kunyeID.KisiID;
                //kisiHane.HaneID = khVM.kunyeHane.kunyeID.HaneID;
            }
            else
            {
                kisiHane = context.tblKisiHane.Find(khVM.id);
            }

            
            kisiHane.KisiID = khVM.kunyeKisi.kunyeID.KisiID;
            kisiHane.HaneID = khVM.kunyeHane.kunyeID.HaneID;
            kisiHane.BasTar = khVM.BasTar;
            kisiHane.BitTar = khVM.BitTar; ;

            if (kisiHane.id == 0)
            {
                context.tblKisiHane.Add(kisiHane);
                m = new Mesaj("bilgi", "Kişi haneye kaydedildi");
                mesajlar.Add(m);
            }
            else
            {
                context.Entry(kisiHane).State = EntityState.Modified;
                m = new Mesaj("bilgi", "Kişi haneye kayıt tarihi değiştirildi");
                mesajlar.Add(m);
            }

            context.SaveChanges();

            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/KisiHane/Index?haneliHanesiz=1", false);

            return Content("OK");
        }

        [ValidateAntiForgeryToken]
        public ActionResult KisiHaneSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KISIHANE kh = context.tblKisiHane.Find(id);
            context.Entry(kh).State = EntityState.Deleted;
            context.SaveChanges();

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Kisi Hane Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Kisi Hane Kaydı Silinemedi");
            }


            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/KisiHane/Index?haneliHanesiz=1", false);

            return Content("OK");
        }

    }
}