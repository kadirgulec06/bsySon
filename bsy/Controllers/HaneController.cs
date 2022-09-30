using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Hane;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{

    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI,SAHAGOREVLISI")]
    public class HaneController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Hane

        public ActionResult ListeMahalleler(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            // filtre parametrelerini hazırla

            User user = (User)Session["USER"];
            string sehir = "";
            string ilce = "";
            string mahalle = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["SEHIR"] != null)
                {
                    sehir = Request.Params["SEHIR"];
                }
                if (Request.Params["ILCE"] != null)
                {
                    ilce = Request.Params["ILCE"];
                }

                if (Request.Params["MAHALLE"] != null)
                {
                    mahalle = Request.Params["MAHALLE"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIR"] = sehir.ToUpper();
                Session["filtreILCE"] = ilce.ToUpper();
                Session["filtreMAHALLE"] = mahalle.ToUpper();
            }
            else if (rapor == 1)
            {
                sehir = (string)Session["filtreSEHIR"];
                ilce = (string)Session["filtreILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //long ilID = (long)Session["ilID"];
            //pageSize = 5;
            var query = (from mh in context.tblMahalleler
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (user.gy.butunTurkiye || user.gy.mahalleler.Contains(mh.id)) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sz.Aciklama + "").Contains(sehir)
                         select new
                         {
                             mh.id,
                             mh.ilceID,
                             ic.sehirID,
                             //mahalleKODU = sx.Kodu,
                             sehirADI = sz.Aciklama,
                             ilceADI = sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             mh.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("sehirADI, ilceADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from mhx in resultSetAfterOrderandPaging
                             select new
                             {
                                 mhx.id,
                                 mhx.ilceID,
                                 mhx.sehirID,
                                 //mhx.mahalleKODU,
                                 mhx.sehirADI,
                                 mhx.ilceADI,
                                 mhx.mahalleADI,
                                 mhx.Aciklama,
                                 Sec = 0,
                                 Hane = 0
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
                  from mhx in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 mhx.id.ToString(),
                                 mhx.ilceID.ToString(),
                                 mhx.sehirID.ToString(),
                                 //mhx.mahalleKODU,
                                 mhx.sehirADI,
                                 mhx.ilceADI,
                                 mhx.mahalleADI,
                                 mhx.Aciklama,
                                 mhx.Sec.ToString(),
                                 mhx.Hane.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeHaneler(string sidx, string sord, int page, int rows, byte ilkGiris=0, long mahalleID = 0)
        {
            User user = (User)Session["USER"];
            string cadde = "";
            string sokak = "";
            string haneKodu = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["CADDE"] != null)
                {
                    cadde = Request.Params["CADDE"];
                }
                if (Request.Params["SOKAK"] != null)
                {
                    sokak = Request.Params["SOKAK"];
                }

                if (Request.Params["HANEKODU"] != null)
                {
                    haneKodu = Request.Params["HANEKODU"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreCADDE"] = cadde.ToUpper();
                Session["filtreSOKAK"] = sokak.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
            }
            else if (rapor == 1)
            {
                cadde = (string)Session["filtreCADDE"];
                sokak = (string)Session["filtreSOKAK"];
                haneKodu = (string)Session["filtreHANEKODU"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from h in context.tblHaneler
                         where 
                            (h.MahalleID == mahalleID || mahalleID == 0) &&
                            (h.Cadde + "").Contains(cadde) &&
                            (h.Sokak + "").Contains(sokak) &&
                            (h.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             h.Apartman,
                             h.BrutAlan,
                             h.Cadde,
                             h.Daire,
                             h.EkBilgi,
                             h.HaneKodu,
                             h.id,
                             h.KayitTarihi,
                             h.MahalleID,
                             h.OdaSayisi,
                             h.Sokak
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("Ad, Soyad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from k in resultSetAfterOrderandPaging
                             select new
                             {
                                 k.id,
                                 k.MahalleID,
                                 k.HaneKodu,
                                 k.KayitTarihi,
                                 k.Cadde,
                                 k.Sokak,
                                 ApartmanDaire = k.Apartman + "-" + k.Daire,
                                 k.EkBilgi,
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
                  from k in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 k.id.ToString(),
                                 k.MahalleID.ToString(),
                                 k.HaneKodu,
                                 k.KayitTarihi.ToShortDateString(),
                                 k.Cadde,
                                 k.Sokak,
                                 k.ApartmanDaire,
                                 k.EkBilgi,
                                 k.Degistir.ToString(),
                                 k.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            Session["mahalleSec"] = 0;
            Session["mahalleID"] = 0;

            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult Mahalle(long mahalleID, int mahalleSec=0)
        {
            Session["mahalleSec"] = mahalleSec;
            Session["mahalleID"] = mahalleID;

            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult YeniHane(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            HANE hane = null;
            try
            {
                hane = context.tblHaneler.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (hane == null)
            {
                hane = new HANE();
            }

            HaneVM haneVM = haneHazirla(hane);

            return View(haneVM);
        }

        private HaneVM haneHazirla(HANE hane)
        {
            long mahalleID = (long)Session["mahalleID"];
            HaneVM haneVM = new HaneVM();
            haneVM.hane = hane;

            if (hane.id != 0)
            {
                haneVM.kunye = SozlukHelper.KunyeHazirla(context, hane.id);
            }
            else
            {
                haneVM.kunye = SozlukHelper.KunyeHazirla(context, mahalleID);
            }

            return haneVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniHane(HaneVM yeniHane, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniHane);
            }

            HANE hane = context.tblHaneler.Find(yeniHane.hane.id);
            if (hane == null)
            {
                hane = new HANE();
            }

            HaneVM eskiHane = haneHazirla(hane);

            bool yeniHaneIslemi = false;
            bool kaydedildi = true;
            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiHane = HaneYeniToEski(eskiHane, yeniHane);
                    if (eskiHane.hane.id == 0)
                    {
                        yeniHaneIslemi = true;
                        hane = context.tblHaneler.Where(kx => kx.HaneKodu == eskiHane.hane.HaneKodu).FirstOrDefault();
                        if (hane != null)
                        {
                            m = new Mesaj("hata", "Bu Hane Kodu daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniHane);
                        }
                        else
                        {
                            context.tblHaneler.Add(eskiHane.hane);
                            m = new Mesaj("tamam", "Hane Kaydı Eklenmiştir.");
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        context.Entry(eskiHane.hane).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Hane Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                        //bool gonderildi = epostaGonder(eskiHane);               
                    }
                    catch (Exception e)
                    {
                        kaydedildi = false;
                        m = new Mesaj("hata", "Hane kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
                    }
                }
                catch (Exception ex)
                {
                    kaydedildi = false;
                    m = new Mesaj("hata", "Hata oluştu: " + GenelHelper.exceptionMesaji(ex));
                    mesajlar.Add(m);
                    Session["MESAJLAR"] = mesajlar;
                }
            }

            mesajlar.Add(m);

            Session["MESAJLAR"] = mesajlar;

            int mahalleSec = (int)Session["mahalleSec"];
            long mahalleID = (long)Session["mahalleID"];

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Haneler/Mahalle?mahalleID="+mahalleID.ToString()+ "&mahalleSec="+mahalleSec.ToString(), false);
            return Content("OK");

        }

        private HaneVM HaneYeniToEski(HaneVM eskiHane, HaneVM yeniHane)
        {
            eskiHane.kunye.HaneID = yeniHane.hane.id;
            eskiHane.kunye.HaneKODU = yeniHane.hane.HaneKodu;

            eskiHane.hane.id = yeniHane.hane.id;
            eskiHane.hane.Apartman = yeniHane.hane.Apartman;
            eskiHane.hane.BrutAlan = yeniHane.hane.BrutAlan;
            eskiHane.hane.Cadde = yeniHane.hane.Cadde;
            eskiHane.hane.Daire = yeniHane.hane.Daire;
            eskiHane.hane.EkBilgi = yeniHane.hane.EkBilgi;
            eskiHane.hane.HaneKodu = yeniHane.hane.HaneKodu;
            eskiHane.hane.KayitTarihi = yeniHane.hane.KayitTarihi;
            eskiHane.hane.MahalleID = eskiHane.kunye.MahalleID;
            eskiHane.hane.OdaSayisi = yeniHane.hane.OdaSayisi;
            eskiHane.hane.Sokak = yeniHane.hane.Sokak;

            return eskiHane;
        }

        [ValidateAntiForgeryToken]
        public ActionResult HaneSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            HANE hane = context.tblHaneler.Find(id);
            context.Entry(hane).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Hane Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Hane Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            int mahalleSec = (int)Session["mahalleSec"];
            long mahalleID = (long)Session["mahalleID"];

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Haneler/Mahalle?mahalleID=" + mahalleID.ToString() + "&mahalleSec=" + mahalleSec.ToString(), false);

            return Content("OK");
        }
    }
}