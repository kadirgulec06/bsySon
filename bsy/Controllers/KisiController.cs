using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Kisi;
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
    public class KisiController : Controller
    {
        bsyContext context = new bsyContext();
        // GET: Kisi
        public ActionResult ListeHaneler(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
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
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id)) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (sy.Aciklama + "").Contains(ilce) &&
                            (sz.Aciklama + "").Contains(sehir) &&
                            (hn.HaneKodu + "").Contains(haneKodu)
                         select new
                         {
                             hn.id,
                             hn.HaneKodu,
                             mahalleKODU = mh.MahalleKodu,
                             sehirADI = sz.Aciklama,
                             ilceADI = sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             hn.Cadde,
                             hn.Sokak,
                             ApartmanDaire=hn.Apartman + "-" + hn.Daire
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("sehirADI, ilceADI").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from hx in resultSetAfterOrderandPaging
                             select new
                             {
                                 hx.id,
                                 hx.HaneKodu,
                                 hx.sehirADI,
                                 hx.ilceADI,
                                 hx.mahalleADI,
                                 hx.Cadde,
                                 hx.Sokak,
                                 hx.ApartmanDaire,
                                 Sec = 0,
                                 Kisi = 0
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
                                 hx.Sec.ToString(),
                                 hx.Kisi.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeKisiler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long haneID = 0)
        {
            User user = (User)Session["USER"];

            string sehir = "";
            string ilce = "";
            string mahalle = "";
            string haneKodu = "";
            string cadde = "";
            string sokak = "";
            string apartman = "";
            string ad = "";
            string soyad = "";
            string tcNo = "";

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

                if (Request.Params["AD"] != null)
                {
                    ad = Request.Params["AD"];
                }
                if (Request.Params["SOYAD"] != null)
                {
                    soyad = Request.Params["SOYAD"];
                }

                if (Request.Params["TCNO"] != null)
                {
                    tcNo = Request.Params["TCNO"];
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
                Session["filtreAD"] = ad.ToUpper();
                Session["filtreSOYAD"] = soyad.ToUpper();
                Session["filtreTCNO"] = tcNo.ToUpper();
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
                ad = (string)Session["filtreAD"];
                soyad = (string)Session["filtreSOYAD"];
                tcNo = (string)Session["filtreTCNO"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;

            var sonKisiHane = (from kh in context.tblKisiHane
                            join hn in context.tblHaneler on kh.HaneID equals hn.id
                            join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                            where
                              (kh.HaneID == haneID || haneID == 0) &&
                              (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                            group kh by kh.BasTar into khGRP
                            select khGRP.OrderByDescending(g => g.BasTar).FirstOrDefault());

            var query = (from kx in context.tblKisiler
                         join kh in sonKisiHane on kx.id equals kh.KisiID
                         join hn in context.tblHaneler on kh.HaneID equals hn.id
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
                            (hn.Apartman + "").Contains(apartman) &&
                            (kx.Ad + "").Contains(ad) &&
                            (kx.Soyad + "").Contains(soyad) &&
                            (kx.TCNo + "").Contains(tcNo)
                         select new
                         {
                             kx.id,
                             hn.HaneKodu,
                             sehirADI = sz.Aciklama,
                             ilceADI = sy.Aciklama,
                             mahalleADI = sm.Aciklama,
                             hn.Cadde,
                             hn.Sokak,
                             ApartmanDaire = hn.Apartman + "-" + hn.Daire,
                             kx.Ad,
                             kx.Soyad,
                             kx.TCNo
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("Ad, Soyad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kx in resultSetAfterOrderandPaging
                             select new
                             {
                                 kx.id,
                                 kx.HaneKodu,
                                 kx.sehirADI,
                                 kx.ilceADI,
                                 kx.mahalleADI,
                                 kx.Cadde,
                                 kx.Sokak,
                                 kx.ApartmanDaire,
                                 kx.Ad,
                                 kx.Soyad,
                                 kx.TCNo,
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
                  from kx in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 kx.id.ToString(),
                                 kx.HaneKodu,
                                 kx.sehirADI,
                                 kx.ilceADI,
                                 kx.mahalleADI,
                                 kx.Cadde,
                                 kx.Sokak,
                                 kx.ApartmanDaire,
                                 kx.Ad,
                                 kx.Soyad,
                                 kx.TCNo,
                                 kx.Degistir.ToString(),
                                 kx.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            Session["HaneID"] = 0;
            Session["HaneSec"] = 0;

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


        public ActionResult YeniKisi(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KISI kisi = null;
            try
            {
                kisi = context.tblKisiler.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (kisi == null)
            {
                kisi = new KISI();
            }

            KisiVM kisiVM = kisiHazirla(kisi);

            return View(kisiVM);
        }

        private KisiVM kisiHazirla(KISI kisi)
        {
            long haneID = (long)Session["haneID"];
            KisiVM kisiVM = new KisiVM();
            kisiVM.kisi = kisi;

            if (kisi.id != 0)
            {
                kisiVM.kunye = SozlukHelper.KunyeHazirla(context, kisi.id);
                kisiVM.kisiHane = context.tblKisiHane
                    .Where(kh => kh.KisiID == kisi.id && kh.HaneID == kisiVM.kunye.HaneID)
                    .OrderByDescending(kx => kx.BasTar).FirstOrDefault();
            }
            else
            {
                kisiVM.kunye = SozlukHelper.KunyeHazirla(context, haneID);
                kisiVM.kisiHane = new KISIHANE();               
            }

            kisiVM = listeleriHazirla(kisiVM);
            
            return kisiVM;
        }

        private KisiVM listeleriHazirla(KisiVM kisiVM)
        {
            kisiVM.cinsiyetler = SozlukHelper.sozlukKalemleriListesi(context, "CINSIYET", kisiVM.kisi.Cinsiyet);

            return kisiVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniKisi(KisiVM yeniKisi, string btnSubmit)
        {
            //yeniKisi = listeleriHazirla(yeniKisi);
            
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniKisi);
            }

            if (!TCKimlikNoValidationAttribute.ValidateKimlikNo(yeniKisi.kisi.TCNo.ToString()))
            {
                m = new Mesaj("hata", "TC Kimlik Numarası Hatalı");
                mesajlar.Add(m);
                Session["MESAJLAR"] = mesajlar;
                return View(yeniKisi);
            }

            KISI kisi = context.tblKisiler.Find(yeniKisi.kisi.id);
            if (kisi == null)
            {
                kisi = new KISI();
            }

            KisiVM eskiKisi = kisiHazirla(kisi);

            bool yeniKisiIslemi = false;
            bool kaydedildi = true;
            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiKisi = KisiYeniToEski(eskiKisi, yeniKisi);
                    if (eskiKisi.kisi.id == 0)
                    {
                        yeniKisiIslemi = true;
                        kisi = context.tblKisiler.Where(kx => kx.TCNo == eskiKisi.kisi.TCNo).FirstOrDefault();
                        if (kisi != null)
                        {
                            m = new Mesaj("hata", "Bu Kisi(TC Numarası) daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniKisi);
                        }
                        else
                        {
                            SOZLUK sozluk = SozlukHelper.kisiSozlugu(context, eskiKisi.kisi);
                            context.tblSozluk.Add(sozluk);
                            context.SaveChanges();

                            eskiKisi.kisi.id = sozluk.id; // SozlukHelper.sozlukID(context, eskiIlceVM.sozluk);
                            context.tblKisiler.Add(eskiKisi.kisi);

                            eskiKisi.kisiHane.KisiID = eskiKisi.kisi.id;
                            eskiKisi.kisiHane.HaneID = eskiKisi.kunye.HaneID;
                            eskiKisi.kisiHane.BasTar = eskiKisi.kisi.KayitTarihi;
                            context.tblKisiHane.Add(eskiKisi.kisiHane);
                            //context.SaveChanges();

                            m = new Mesaj("tamam", "Kisi Kaydı Eklenmiştir.");
                            
                        }
                    }
                    else
                    {
                        SOZLUK sozluk = SozlukHelper.kisiSozlugu(context, eskiKisi.kisi);
                        context.Entry(sozluk).State = EntityState.Modified;
                        context.Entry(eskiKisi.kisi).State = EntityState.Modified;
                        context.Entry(eskiKisi.kisiHane).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Kisi Kaydı Güncellenmiştir.");
                    }
                    try
                    {
                        context.SaveChanges();
                        scope.Complete();
                        //bool gonderildi = epostaGonder(eskiKisi);               
                    }
                    catch (Exception e)
                    {
                        kaydedildi = false;
                        m = new Mesaj("hata", "Kisi kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
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

            int haneSec = (int)Session["haneSec"];
            long haneID = (long)Session["haneID"];

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kisi/Hane?haneID=" + haneID.ToString() + "&haneSec=" + haneSec.ToString(), false);
            return Content("OK");

        }

        private KisiVM KisiYeniToEski(KisiVM eskiKisi, KisiVM yeniKisi)
        {
            eskiKisi.kunye.KisiID = yeniKisi.kisi.id;

            eskiKisi.kisi.id = yeniKisi.kisi.id;
            eskiKisi.kisi.TCNo = yeniKisi.kisi.TCNo;
            eskiKisi.kisi.KayitTarihi = yeniKisi.kisi.KayitTarihi;
            eskiKisi.kisi.Ad = yeniKisi.kisi.Ad;
            eskiKisi.kisi.Soyad = yeniKisi.kisi.Soyad;
            eskiKisi.kisi.Cinsiyet = yeniKisi.kisi.Cinsiyet;
            eskiKisi.kisi.Telefon = yeniKisi.kisi.Telefon;
            eskiKisi.kisi.Eposta = yeniKisi.kisi.Eposta;
            eskiKisi.kisi.EkBilgi = yeniKisi.kisi.EkBilgi;

            eskiKisi.kisiHane.BasTar = yeniKisi.kisi.KayitTarihi;

            return eskiKisi;
        }

        [ValidateAntiForgeryToken]
        public ActionResult KisiSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KISI kisi = context.tblKisiler.Find(id);
            context.Entry(kisi).State = EntityState.Deleted;
            context.SaveChanges();

            SOZLUK kisiSozluk = context.tblSozluk.Find(id);
            context.Entry(kisiSozluk).State = EntityState.Deleted;
            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Kisi Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Kisi Kaydı Silinemedi");
            }


            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            int haneSec = (int)Session["haneSec"];
            long haneID = (long)Session["haneID"];

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kisi/Hane?haneID=" + haneID.ToString() + "&haneSec=" + haneSec.ToString(), false);

            return Content("OK");
        }
    }
}