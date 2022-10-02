using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.HaneGorusme;
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
    public class HaneGorusmeController : Controller
    {
        bsyContext context = new bsyContext();

        // GET: HaneGorusme
        public ActionResult ListeHaneler(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
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
                            (user.gy.butunTurkiye == true || 
                            user.gy.mahalleler.Contains(h.MahalleID)) &&
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
                                 Sec = 0,
                                 Gorusmeler = 0
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
                                 h.Sec.ToString(),
                                 h.Gorusmeler.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeGorusmeler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long haneID = 0)
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
            var query = (from hg in context.tblHaneGorusme
                         join hn in context.tblHaneler on hg.HaneID equals hn.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (hg.HaneID == haneID || haneID == 0) &&
                            (user.gy.butunTurkiye == true ||
                            user.gy.mahalleler.Contains(hn.MahalleID)) &&
                            (sz.Aciklama + "").Contains(sehir) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (hn.Cadde + "").Contains(cadde) &&
                            (hn.Sokak + "").Contains(sokak) &&
                            (hn.Apartman + "").Contains(apartman) &&
                            (hn.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             hg.id,
                             haneID = hn.id,
                             hn.HaneKodu,
                             sehirADI = sz.Aciklama,
                             ilceADI = sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             hn.Cadde,
                             hn.Sokak,
                             ApartmanDaire = hn.Apartman + "-" + hn.Daire,
                             hg.GorusmeTarihi,
                             hg.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("HaneKodu").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from hg in resultSetAfterOrderandPaging
                             select new
                             {
                                 hg.id,
                                 hg.haneID,
                                 hg.HaneKodu,
                                 hg.sehirADI,
                                 hg.ilceADI,
                                 hg.mahalleADI,
                                 hg.Cadde,
                                 hg.Sokak,
                                 hg.ApartmanDaire,
                                 hg.GorusmeTarihi,
                                 hg.Aciklama,
                                 Ekle=0,
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
                  from hg in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 hg.id.ToString(),
                                 hg.haneID.ToString(),
                                 hg.HaneKodu,
                                 hg.sehirADI,
                                 hg.ilceADI,
                                 hg.mahalleADI,
                                 hg.Cadde,
                                 hg.Sokak,
                                 hg.ApartmanDaire,
                                 hg.GorusmeTarihi.ToShortDateString(),
                                 hg.Aciklama,
                                 hg.Ekle.ToString(),
                                 hg.Degistir.ToString(),
                                 hg.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            Session["haneSec"] = 0;
            Session["haneID"] = 0;

            ViewBag.IlkGiris = 1;
            ViewBag.haneID = 0;

            return View();
        }

        public ActionResult Hane(long haneID, int haneSec = 0)
        {
            Session["haneSec"] = haneSec;
            Session["haneID"] = haneID;

            ViewBag.IlkGiris = 1;
            ViewBag.haneID = haneID;

            return View();
        }

        public ActionResult YeniHaneGorusme(long id = 0, int yeniGorusme=0)
        {                           
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            HANEGORUSME hane = null;
            try
            {
                hane = context.tblHaneGorusme.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (hane == null)
            {
                hane = new HANEGORUSME();
            }

            HaneGorusmeVM haneVM = haneGorusmeHazirla(hane);
            haneVM.yeniGorusme = yeniGorusme;

            return View(haneVM);
        }

        private HaneGorusmeVM haneGorusmeHazirla(HANEGORUSME hane)
        {
            long haneID = (long)Session["haneID"];
            HaneGorusmeVM haneVM = new HaneGorusmeVM();

            if (hane.HaneID == 0)
            {
                hane.HaneID = haneID;
            }
            else
            {
                haneID = hane.HaneID;
            }

            haneVM.haneGorusme = hane;

            haneVM.kunye = SozlukHelper.KunyeHazirla(context, haneID);

            haneVM.Ihtiyaclar = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ihtiyaclarTuru, hane.Ihtiyaclar, true);
            haneVM.BelediyeYardimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.belediyeYardimiTuru, hane.BelediyeYardimi, true);
            haneVM.EvMulkiyeti = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evMulkiyetiTuru, hane.EvMulkiyeti, true);
            haneVM.EvTuru = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evTuru, hane.EvTuru, true);
            haneVM.HaneGelirDilimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.gelirDilimiTuru, hane.HaneGelirDilimi, true);

            return haneVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniHaneGorusme(HaneGorusmeVM yeniHane, string btnSubmit)
        {
            int haneSec = 0;
            long haneID = 0;
            try
            {
                haneSec = (int)Session["haneSec"];
                haneID = (long)Session["haneID"];
            }
            catch { }

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniHane);
            }

            HANEGORUSME hane = context.tblHaneGorusme.Find(yeniHane.haneGorusme.id);
            if (hane == null)
            {
                hane = new HANEGORUSME();
            }

            HaneGorusmeVM eskiHane = haneGorusmeHazirla(hane);

            bool yeniHaneIslemi = false;
            bool kaydedildi = true;
            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiHane = HaneYeniToEski(eskiHane, yeniHane);
                    if (eskiHane.haneGorusme.id == 0 || eskiHane.yeniGorusme == 1)
                    {
                        yeniHaneIslemi = true;
                        eskiHane.haneGorusme.id = 0;
                        hane = context.tblHaneGorusme.Where(kx => kx.HaneID == eskiHane.haneGorusme.HaneID &&
                                                            kx.GorusmeTarihi == eskiHane.haneGorusme.GorusmeTarihi).FirstOrDefault();
                        if (hane != null)
                        {
                            m = new Mesaj("hata", "Bu Tarihli Hane Görüşme Kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniHane);
                        }
                        else
                        {
                            context.tblHaneGorusme.Add(eskiHane.haneGorusme);
                            m = new Mesaj("tamam", "Hane Görüşme Kaydı Eklenmiştir.");
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        context.Entry(eskiHane.haneGorusme).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Hane Görüşme Kaydı Güncellenmiştir.");
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
                        m = new Mesaj("hata", "Hane Görüşme kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/HaneGorusme/Hane?haneID=" + haneID.ToString() + "&haneSec=" + haneSec.ToString(), false);
            return Content("OK");

        }

        private HaneGorusmeVM HaneYeniToEski(HaneGorusmeVM eskiHane, HaneGorusmeVM yeniHane)
        {
            if (yeniHane.haneGorusme.HaneID != 0)
            {
                eskiHane.kunye.HaneID = yeniHane.haneGorusme.HaneID;
            }

            eskiHane.yeniGorusme = yeniHane.yeniGorusme;
            eskiHane.haneGorusme.id = yeniHane.haneGorusme.id;
            eskiHane.haneGorusme.HaneID = yeniHane.kunye.HaneID;
            eskiHane.haneGorusme.GorusmeTarihi = yeniHane.haneGorusme.GorusmeTarihi;
            eskiHane.haneGorusme.Aciklama = yeniHane.haneGorusme.Aciklama;
            eskiHane.haneGorusme.Ihtiyaclar = yeniHane.haneGorusme.Ihtiyaclar;
            eskiHane.haneGorusme.BelediyeYardimi = yeniHane.haneGorusme.BelediyeYardimi;
            eskiHane.haneGorusme.EvTuru = yeniHane.haneGorusme.EvTuru;
            eskiHane.haneGorusme.EvMulkiyeti = yeniHane.haneGorusme.EvMulkiyeti;
            eskiHane.haneGorusme.KiraTutari = yeniHane.haneGorusme.KiraTutari;
            eskiHane.haneGorusme.EkBilgi = yeniHane.haneGorusme.EkBilgi;
            eskiHane.haneGorusme.HaneGelirDilimi = yeniHane.haneGorusme.HaneGelirDilimi;

            return eskiHane;
        }

        [ValidateAntiForgeryToken]
        public ActionResult HaneSil(long idSil)
        {
            int haneSec = 0;
            long haneID = 0;
            try
            {
                haneSec = (int)Session["haneSec"];
                haneID = (long)Session["haneID"];
            }
            catch { }

            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            HANEGORUSME hane = context.tblHaneGorusme.Find(id);
            context.Entry(hane).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Hane Görüşme Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Hane Görüşme Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/HaneGorusme/Hane?haneID=" + haneID.ToString() + "&haneSec=" + haneSec.ToString(), false);

            return Content("OK");
        }
    }
}