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
                            (user.gy.butunTurkiye == true || 
                            user.gy.mahalleler.Contains(h.MahalleID)) &&
                            (sz.Aciklama + " " + sy.Aciklama + "").Contains(sehirIlce) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (h.Cadde + " " + h.Sokak + " " + h.Apartman + "-" + h.Daire + "").Contains(adres) &&
                            (h.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             h.id,
                             h.HaneKodu,
                             sehirIlce = sz.Aciklama + "-" + sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             Adres = h.Cadde +  " " + h.Sokak + " "+ h.Apartman + " "+ h.Daire
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
                                 h.sehirIlce,
                                 h.mahalleADI,
                                 h.Adres,
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
                            (sz.Aciklama + " " + sy.Aciklama + "").Contains(sehirIlce) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + " " + hn.Daire + "").Contains(adres) &&
                            (hn.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             hg.id,
                             haneID = hn.id,
                             hn.HaneKodu,
                             sehirIlce = sz.Aciklama + "-" + sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             Adres = hn.Cadde + " " + hn.Sokak + " "+ hn.Apartman + "-" + hn.Daire,
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
                                 hg.sehirIlce,
                                 hg.mahalleADI,
                                 hg.Adres,
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
                                 hg.sehirIlce,
                                 hg.mahalleADI,
                                 hg.Adres,
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

            HaneGorusmeVM haneVM = haneGorusmeHazirla(hane, yeniGorusme);

            return View(haneVM);
        }

        private HaneGorusmeVM haneGorusmeHazirla(HANEGORUSME hane, int yeniGorusme)
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

            if (hane.id != 0 && yeniGorusme == 1)
            {
                hane = HaneToHane(hane);
            }

            haneVM.haneGorusme = hane;
            haneVM.yeniGorusme = yeniGorusme;

            haneVM.kunye = SozlukHelper.KunyeHazirla(context, haneID);

            haneVM = listeleriHazirla(haneVM);

            return haneVM;
        }

        private HaneGorusmeVM listeleriHazirla(HaneGorusmeVM hgVM)
        {
            hgVM.haneListeleri.Ihtiyaclar = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ihtiyaclarTuru, hgVM.haneGorusme.Ihtiyaclar, 2);
            hgVM.haneListeleri.BelediyeYardimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.belediyeYardimiTuru, hgVM.haneGorusme.BelediyeYardimi, 2);
            hgVM.haneListeleri.EvMulkiyeti = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evMulkiyetiTuru, hgVM.haneGorusme.EvMulkiyeti, 2);
            hgVM.haneListeleri.EvTuru = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evTuru, hgVM.haneGorusme.EvTuru, 2);
            hgVM.haneListeleri.HaneGelirDilimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.gelirDilimiTuru, hgVM.haneGorusme.HaneGelirDilimi, 2);

            return hgVM;
        }

        private HANEGORUSME HaneToHane(HANEGORUSME eskiHG)
        {
            HANEGORUSME yeniHG = new HANEGORUSME();

            yeniHG.Aciklama = eskiHG.Aciklama;
            yeniHG.BelediyeYardimi = eskiHG.BelediyeYardimi;
            yeniHG.EkBilgi = eskiHG.EkBilgi;
            yeniHG.EvMulkiyeti = eskiHG.EvMulkiyeti;
            yeniHG.EvTuru = eskiHG.EvTuru;
            yeniHG.GorusmeTarihi = DateTime.Now.Date;
            yeniHG.HaneGelirDilimi = eskiHG.HaneGelirDilimi;
            yeniHG.HaneID = eskiHG.HaneID;
            yeniHG.id = 0;
            yeniHG.Ihtiyaclar = eskiHG.Ihtiyaclar;
            yeniHG.KiraTutari = eskiHG.KiraTutari;

            return yeniHG;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniHaneGorusme(HaneGorusmeVM yeniHane, string btnSubmit)
        {
            yeniHane = listeleriHazirla(yeniHane);

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

            HaneGorusmeVM eskiHane = haneGorusmeHazirla(hane, yeniHane.yeniGorusme);

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
                eskiHane.kunye.kunyeID.HaneID = yeniHane.haneGorusme.HaneID;
            }

            eskiHane.yeniGorusme = yeniHane.yeniGorusme;
            eskiHane.haneGorusme.id = yeniHane.haneGorusme.id;
            eskiHane.haneGorusme.HaneID = yeniHane.kunye.kunyeID.HaneID;
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