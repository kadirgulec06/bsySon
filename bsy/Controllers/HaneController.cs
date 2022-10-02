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
                            (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id)) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sz.Aciklama + "").Contains(sehir)
                         select new
                         {
                             mh.id,
                             mh.ilceID,
                             ic.sehirID,
                             mahalleKODU=mh.MahalleKodu,
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
                                 mhx.mahalleKODU,
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
                                 //mhx.ilceID.ToString(),
                                 //mhx.sehirID.ToString(),
                                 mhx.sehirADI,
                                 mhx.ilceADI,
                                 //mhx.mahalleKODU,
                                 mhx.mahalleADI,
                                 //mhx.Aciklama,
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
            string sehir = "";
            string ilce = "";
            string mahalle = "";
            string cadde = "";
            string sokak = "";
            string apartman = "";
            string haneKodu = "";

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

                if (Request.Params["CADDE"] != null)
                {
                    cadde = Request.Params["CADDE"];
                }

                if (Request.Params["SOKAK"] != null)
                {
                    sokak = Request.Params["SOKAK"];
                }

                if (Request.Params["APARTMAN"] != null)
                {
                    apartman = Request.Params["APARTMAN"];
                }

                if (Request.Params["HANEKODU"] != null)
                {
                    haneKodu = Request.Params["HANEKODU"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIR"] = sehir.ToUpper();
                Session["filtreILCE"] = ilce.ToUpper();
                Session["filtreMAHALLE"] = mahalle.ToUpper();
                Session["filtreCADDE"] = cadde.ToUpper();
                Session["filtreSOKAK"] = sokak.ToUpper();
                Session["filtreAPARTMAN"] = apartman.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
            }
            else if (rapor == 1)
            {
                sehir = (string)Session["filtreSEHIR"];
                ilce = (string)Session["filtreILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
                cadde = (string)Session["filtreCADDE"];
                sokak = (string)Session["filtreSOKAK"];
                apartman = (string)Session["filtreAPARTMAN"];
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
                            (
                            h.MahalleID == mahalleID || 
                            mahalleID == 0 &&
                            (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(h.MahalleID))
                            ) &&
                            (sz.Aciklama + "").Contains(sehir) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (h.Cadde + "").Contains(cadde) &&
                            (h.Sokak + "").Contains(sokak) &&
                            (h.Apartman + "").Contains(apartman) &&
                            (h.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             h.id,
                             h.HaneKodu,
                             sehirADI = sz.Aciklama,
                             ilceADI = sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             h.Cadde,
                             h.Sokak,
                             ApartmanDaire = h.Apartman + "-" + h.Daire
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("HaneKodu").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from h in resultSetAfterOrderandPaging
                             select new
                             {
                                 h.id,
                                 h.HaneKodu,
                                 h.sehirADI,
                                 h.ilceADI,
                                 h.mahalleADI,
                                 h.Cadde,
                                 h.Sokak,
                                 h.ApartmanDaire,
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
                  from h in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 h.id.ToString(),
                                 h.HaneKodu,
                                 h.sehirADI,
                                 h.ilceADI,
                                 h.mahalleADI,
                                 h.Cadde,
                                 h.Sokak,
                                 h.ApartmanDaire,
                                 h.Degistir.ToString(),
                                 h.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeHanelerYENI(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            // filtre parametrelerini hazırla

            User user = (User)Session["USER"];

            string sehir = "";
            string ilce = "";
            string mahalle = "";
            string haneKodu = "";
            string cadde = "";
            string sokak = "";
            string apartman = "";

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

                if (Request.Params["HANEKODU"] != null)
                {
                    haneKodu = Request.Params["HANEKODU"];
                }

                if (Request.Params["CADDE"] != null)
                {
                    cadde = Request.Params["CADDE"];
                }

                if (Request.Params["SOKAK"] != null)
                {
                    sokak = Request.Params["SOKAK"];
                }

                if (Request.Params["APARTMAN"] != null)
                {
                    apartman = Request.Params["APARTMAN"];
                }
            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreSEHIR"] = sehir.ToUpper();
                Session["filtreILCE"] = ilce.ToUpper();
                Session["filtreMAHALLE"] = mahalle.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
                Session["filtreCADDE"] = cadde.ToUpper();
                Session["filtreSOKAK"] = sokak.ToUpper();
                Session["filtreAPARTMAN"] = apartman.ToUpper();
            }
            else if (rapor == 1)
            {
                sehir = (string)Session["filtreSEHIR"];
                ilce = (string)Session["filtreILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
                haneKodu = (string)Session["filtreHANEKODU"];
                cadde = (string)Session["filtreCADDE"];
                sokak = (string)Session["filtreSOKAK"];
                apartman = (string)Session["filtreAPARTMAN"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //long ilID = (long)Session["ilID"];
            //pageSize = 5;
            var query = (from hn in context.tblHaneler
                         join sx in context.tblSozluk on hn.id equals sx.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sm in context.tblSozluk on mh.id equals sm.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id)) &&
                            (sm.Aciklama + "").Contains(mahalle) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sz.Aciklama + "").Contains(sehir) &&
                            (hn.HaneKodu + "").Contains(haneKodu) &&
                            (hn.Cadde + "").Contains(cadde) &&
                            (hn.Sokak + "").Contains(sokak) &&
                            (hn.Apartman + "").Contains(apartman)
                         select new
                         {
                             hn.id,
                             hn.HaneKodu,
                             mahalleKODU = mh.MahalleKodu,
                             sehirADI = sz.Aciklama,
                             ilceADI = sy.Aciklama,
                             mahalleADI = sm.Aciklama,
                             hn.Cadde,
                             hn.Sokak,
                             ApartmanDaire = hn.Apartman + "-" + hn.Daire
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("sehirADI, ilceADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from hx in resultSetAfterOrderandPaging
                             select new
                             {
                                 hx.id,
                                 hx.HaneKodu,
                                 hx.mahalleKODU,
                                 hx.sehirADI,
                                 hx.ilceADI,
                                 hx.mahalleADI,
                                 hx.Cadde,
                                 hx.Sokak,
                                 hx.ApartmanDaire,
                                 Gorusmeler=0,
                                 Kisiler=0,
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
                  from hx in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 hx.id.ToString(),
                                 hx.HaneKodu,
                                 hx.sehirADI,
                                 hx.ilceADI,
                                 hx.mahalleADI,
                                 hx.Cadde,
                                 hx.Sokak,
                                 hx.ApartmanDaire,
                                 hx.Gorusmeler.ToString(),
                                 hx.Kisiler.ToString(),
                                 hx.Degistir.ToString(),
                                 hx.Sil.ToString()
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
            ViewBag.mahalleID = 0;

            return View();
        }

        public ActionResult Mahalle(long mahalleID, int mahalleSec=0)
        {
            Session["mahalleSec"] = mahalleSec;
            Session["mahalleID"] = mahalleID;

            ViewBag.IlkGiris = 1;
            ViewBag.mahalleID = mahalleID;

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

            haneVM.kayitYapildi = 0;

            if (hane.MahalleID == 0)
            {
                hane.MahalleID = mahalleID;
            }
            else
            {
                mahalleID = hane.MahalleID;
            }

            haneVM.hane = hane;
            
            if (hane.id != 0)
            {
                haneVM.kunye = SozlukHelper.KunyeHazirla(context, hane.id);
                haneVM.kayitYapildi = 1;
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
                            SOZLUK sozluk = SozlukHelper.haneSozlugu(context, eskiHane.hane);
                            context.tblSozluk.Add(sozluk);
                            context.SaveChanges();

                            eskiHane.hane.id = sozluk.id; // SozlukHelper.sozlukID(context, eskiIlceVM.sozluk);
                            context.tblHaneler.Add(eskiHane.hane);
                            m = new Mesaj("tamam", "Hane Kaydı Eklenmiştir.");
                            context.SaveChanges();
                            eskiHane.kunye.HaneID = eskiHane.hane.id;
                        }
                    }
                    else
                    {
                        SOZLUK sozluk = SozlukHelper.haneSozlugu(context, eskiHane.hane);
                        context.Entry(sozluk).State = EntityState.Modified;
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

            eskiHane.kayitYapildi = 1;
            return View(eskiHane);

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Hane/Mahalle?mahalleID="+mahalleID.ToString()+ "&mahalleSec="+mahalleSec.ToString(), false);
            return Content("OK");

        }

        private HaneVM HaneYeniToEski(HaneVM eskiHane, HaneVM yeniHane)
        {
            eskiHane.kunye.HaneID = yeniHane.hane.id;
            eskiHane.kunye.HaneKODU = yeniHane.hane.HaneKodu;

            eskiHane.hane.id = yeniHane.hane.id;
            eskiHane.hane.Telefon = yeniHane.hane.Telefon;
            eskiHane.hane.Eposta = yeniHane.hane.Eposta;
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Hane/Mahalle?mahalleID=" + mahalleID.ToString() + "&mahalleSec=" + mahalleSec.ToString(), false);

            return Content("OK");
        }
    }
}