using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.KisiGorusme;
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
    public class KisiGorusmeController : Controller
    {
        bsyContext context = new bsyContext();

        // GET: KisiGorusme
        public ActionResult ListeKisiler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long kisiID = 0)
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
            DateTime bugun = DateTime.Now.Date;

            var sonKisiHane = (from kh in context.tblKisiHane
                               join hn in context.tblHaneler on kh.HaneID equals hn.id
                               join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                               where
                                  kh.BitTar > bugun &&
                                 (kh.HaneID == kisiID || kisiID == 0) &&
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
                            (sy.Aciklama + " " + sz.Aciklama + "").Contains(sehirIlce) &&
                            (hn.HaneKodu + "").Contains(haneKodu) &&
                            (hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + " " + hn.Daire + "").Contains(adres) &&
                            (kx.Ad + " " + kx.Soyad + "").Contains(adSoyad) &&
                            (kx.TCNo + "").Contains(tcNo)
                         select new
                         {
                             kx.id,
                             hn.HaneKodu,
                             sehirIlce = sz.Aciklama + "-" + sy.Aciklama,
                             mahalleADI = sm.Aciklama,
                             Adres = hn.Cadde + " "+ hn.Sokak + " " + hn.Apartman + " " + hn.Daire,
                             AdSoyad = kx.Ad + " " + kx.Soyad,
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
                                 kx.sehirIlce,
                                 kx.mahalleADI,
                                 kx.Adres,
                                 kx.AdSoyad,
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
                                 kx.sehirIlce,
                                 kx.mahalleADI,
                                 kx.Adres,
                                 kx.AdSoyad,
                                 kx.TCNo,
                                 kx.Degistir.ToString(),
                                 kx.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListeGorusmeler(string sidx, string sord, int page, int rows, byte ilkGiris = 0, long kisiID = 0)
        {
            User user = (User)Session["USER"];

            string sehirIlce = "";
            string mahalle = "";
            string adres = "";
            string haneKodu = "";
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

                if (Request.Params["ADRES"] != null)
                {
                    adres = Request.Params["ADRES"];
                }

                if (Request.Params["HANEKODU"] != null)
                {
                    haneKodu = Request.Params["HANEKODU"];
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
                Session["filtreADRES"] = adres.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
                Session["filtreADSOYAD"] = adSoyad.ToUpper();
                Session["filtreTCNO"] = tcNo.ToUpper();
            }
            else if (rapor == 1)
            {
                sehirIlce = (string)Session["filtreSEHIRILCE"];
                mahalle = (string)Session["filtreMAHALLE"];
                adres = (string)Session["filtreADRES"];
                haneKodu = (string)Session["filtreHANEKODU"];
                adSoyad = (string)Session["filtreADSOYAD"];
                tcNo = (string)Session["filtreTCNO"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            DateTime bugun = DateTime.Now.Date;

            var sonKisiHane = (from kh in context.tblKisiHane
                               join hn in context.tblHaneler on kh.HaneID equals hn.id
                               join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                               where
                                  kh.BitTar > bugun &&
                                 (kh.HaneID == kisiID || kisiID == 0) &&
                                 (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                               group kh by kh.BasTar into khGRP
                               select khGRP.OrderByDescending(g => g.BasTar).FirstOrDefault());

            //pageSize = 5;
            var query = (from kg in context.tblKisiGorusme
                         join kx in context.tblKisiler on kg.KisiID equals kx.id
                         join kh in sonKisiHane on kx.id equals kh.KisiID
                         join hn in context.tblHaneler on kh.HaneID equals hn.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.ilceID equals ic.id
                         join sy in context.tblSozluk on mh.ilceID equals sy.id
                         join sh in context.tblSehirler on ic.sehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (kg.KisiID == kisiID || kisiID == 0) &&
                            (user.gy.butunTurkiye == true ||
                            user.gy.mahalleler.Contains(hn.MahalleID)) &&
                            (sz.Aciklama + " " + sy.Aciklama + "").Contains(sehirIlce) &&
                            (sx.Aciklama + "").Contains(mahalle) &&
                            (hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + " " + hn.Daire ).Contains(adres) &&
                            (hn.HaneKodu + "").Contains(haneKodu) &&
                            (kx.Ad + " " +kx.Soyad + "").Contains(adSoyad) &&
                            (kx.TCNo + "").Contains(tcNo)
                         select new
                         {
                             kg.id,
                             kisiID = kx.id,
                             haneID = hn.id,
                             hn.HaneKodu,
                             sehirIlce = sz.Aciklama + "-" + sy.Aciklama,
                             mahalleADI = sx.Aciklama,
                             Adres = hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + "-" + hn.Daire,
                             AdSoyad = kx.Ad + " " + kx.Soyad,
                             kg.GorusmeTarihi,
                             kg.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("KisiKodu").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kg in resultSetAfterOrderandPaging
                             select new
                             {
                                 kg.id,
                                 kg.kisiID,
                                 kg.haneID,
                                 kg.HaneKodu,
                                 kg.sehirIlce,
                                 kg.mahalleADI,
                                 kg.Adres,
                                 kg.AdSoyad,
                                 kg.GorusmeTarihi,
                                 kg.Aciklama,
                                 Ekle = 0,
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
                  from kg in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 kg.id.ToString(),
                                 kg.kisiID.ToString(),
                                 kg.haneID.ToString(),
                                 kg.HaneKodu,
                                 kg.sehirIlce,
                                 kg.mahalleADI,
                                 kg.Adres,
                                 kg.AdSoyad,
                                 kg.GorusmeTarihi.ToShortDateString(),
                                 kg.Aciklama,
                                 kg.Ekle.ToString(),
                                 kg.Degistir.ToString(),
                                 kg.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            Session["kisiSec"] = 0;
            Session["kisiID"] = 0;

            ViewBag.IlkGiris = 1;
            ViewBag.kisiID = 0;

            return View();
        }

        public ActionResult Kisi(long kisiID, int kisiSec = 0)
        {
            Session["kisiSec"] = kisiSec;
            Session["kisiID"] = kisiID;

            ViewBag.IlkGiris = 1;
            ViewBag.kisiID = kisiID;

            return View();
        }

        public ActionResult YeniKisiGorusme(long id = 0, int yeniGorusme = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KISIGORUSME kisi = null;
            try
            {
                kisi = context.tblKisiGorusme.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (kisi == null)
            {
                kisi = new KISIGORUSME();
            }

            KisiGorusmeVM kisiVM = kisiGorusmeHazirla(kisi, yeniGorusme);

            return View(kisiVM);
        }

        private KisiGorusmeVM kisiGorusmeHazirla(KISIGORUSME kisi, int yeniGorusme)
        {
            long kisiID = (long)Session["kisiID"];
            KisiGorusmeVM kisiVM = new KisiGorusmeVM();

            if (kisi.KisiID == 0)
            {
                kisi.KisiID = kisiID;
            }
            else
            {
                kisiID = kisi.KisiID;
            }

            if (kisi.id != 0 && yeniGorusme == 1)
            {
                kisi = KisiToKisi(kisi);
            }

            kisiVM.kisiGorusme = kisi;
            kisiVM.yeniGorusme = yeniGorusme;

            kisiVM.kunye = SozlukHelper.KunyeHazirla(context, kisiID);

            kisiVM = listeleriHazirla(kisiVM);

            return kisiVM;
        }

        private KisiGorusmeVM listeleriHazirla(KisiGorusmeVM kgVM)
        {
            kgVM.kisiListeleri.MedeniDurum = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ihtiyaclarTuru, kgVM.kisiGorusme.MedeniDurumu, 2);
            kgVM.kisiListeleri.EgitimDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.belediyeYardimiTuru, kgVM.kisiGorusme.EgitimDurumu, 2);
            kgVM.kisiListeleri.SosyalGuvence = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evMulkiyetiTuru, kgVM.kisiGorusme.SosyalGuvencesi, 2);
            kgVM.kisiListeleri.SaglikSorunu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evTuru, kgVM.kisiGorusme.SaglikSorunu, 2);
            kgVM.kisiListeleri.CalismaDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.gelirDilimiTuru, kgVM.kisiGorusme.CalismaDurumu, 2);

            kgVM.kisiListeleri.Meslek = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ihtiyaclarTuru, kgVM.kisiGorusme.Meslegi, 2);
            kgVM.kisiListeleri.OkulDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.belediyeYardimiTuru, kgVM.kisiGorusme.OkulDurumu, 2);
            kgVM.kisiListeleri.SosyalDestek = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evMulkiyetiTuru, kgVM.kisiGorusme.SosyalDestek, 2);
            kgVM.kisiListeleri.AsiDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evTuru, kgVM.kisiGorusme.AsiDurumu, 2);
            kgVM.kisiListeleri.OzelDurum = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.gelirDilimiTuru, kgVM.kisiGorusme.OzelDurum, 2);

            kgVM.kisiListeleri.KronikRahatsizlik = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ihtiyaclarTuru, kgVM.kisiGorusme.KronikRahatsizlik, 2);
            kgVM.kisiListeleri.YonlendirmeDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.belediyeYardimiTuru, kgVM.kisiGorusme.YonlendirmeDurumu, 2);
            kgVM.kisiListeleri.OkurYazar = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.evMulkiyetiTuru, kgVM.kisiGorusme.OkurYazar, 2);

            return kgVM;
        }

        private KISIGORUSME KisiToKisi(KISIGORUSME eskiKG)
        {
            KISIGORUSME yeniKG = new KISIGORUSME();

            yeniKG.Aciklama = eskiKG.Aciklama;
            yeniKG.AsiDurumu = eskiKG.AsiDurumu;
            yeniKG.BorcBakkal = eskiKG.BorcBakkal;
            yeniKG.BorcDiger = eskiKG.BorcDiger;
            yeniKG.BorcElektrik = eskiKG.BorcElektrik;
            yeniKG.BorcGaz = eskiKG.BorcGaz;
            yeniKG.BorcInternet = eskiKG.BorcInternet;
            yeniKG.BorcTelefon = eskiKG.BorcTelefon;
            yeniKG.CalismaDurumu = eskiKG.CalismaDurumu;
            yeniKG.EgitimDurumu = eskiKG.EgitimDurumu;
            yeniKG.EkBilgi = eskiKG.EkBilgi;
            yeniKG.GorusmeTarihi = eskiKG.GorusmeTarihi;
            yeniKG.id = 0;
            yeniKG.KisiID = eskiKG.KisiID;
            yeniKG.KronikRahatsizlik = eskiKG.KronikRahatsizlik;
            yeniKG.KronikYuzdesi = eskiKG.KronikYuzdesi;
            yeniKG.MedeniDurumu = eskiKG.MedeniDurumu;
            yeniKG.Meslegi = eskiKG.Meslegi;
            yeniKG.OkulDurumu = eskiKG.OkulDurumu;
            yeniKG.OkurYazar = eskiKG.OkurYazar;
            yeniKG.OzelDurum = eskiKG.OzelDurum;
            yeniKG.SaglikSorunu = eskiKG.SaglikSorunu;
            yeniKG.SosyalDestek = eskiKG.SosyalDestek;
            yeniKG.SosyalGuvencesi = eskiKG.SosyalGuvencesi;
            yeniKG.YonlendirmeDurumu = eskiKG.YonlendirmeDurumu;

            return yeniKG;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniKisiGorusme(KisiGorusmeVM yeniKisi, string btnSubmit)
        {
            yeniKisi = listeleriHazirla(yeniKisi);

            int kisiSec = 0;
            long kisiID = 0;
            try
            {
                kisiSec = (int)Session["kisiSec"];
                kisiID = (long)Session["kisiID"];
            }
            catch { }

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniKisi);
            }

            KISIGORUSME kisi = context.tblKisiGorusme.Find(yeniKisi.kisiGorusme.id);
            if (kisi == null)
            {
                kisi = new KISIGORUSME();
            }

            KisiGorusmeVM eskiKisi = kisiGorusmeHazirla(kisi, yeniKisi.yeniGorusme);

            bool yeniKisiIslemi = false;
            bool kaydedildi = true;
            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiKisi = KisiYeniToEski(eskiKisi, yeniKisi);
                    if (eskiKisi.kisiGorusme.id == 0 || eskiKisi.yeniGorusme == 1)
                    {
                        yeniKisiIslemi = true;
                        eskiKisi.kisiGorusme.id = 0;
                        kisi = context.tblKisiGorusme.Where(kx => kx.KisiID == eskiKisi.kisiGorusme.KisiID &&
                                                            kx.GorusmeTarihi == eskiKisi.kisiGorusme.GorusmeTarihi).FirstOrDefault();
                        if (kisi != null)
                        {
                            m = new Mesaj("hata", "Bu Tarihli Kisi Görüşme Kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniKisi);
                        }
                        else
                        {
                            context.tblKisiGorusme.Add(eskiKisi.kisiGorusme);
                            m = new Mesaj("tamam", "Kisi Görüşme Kaydı Eklenmiştir.");
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        context.Entry(eskiKisi.kisiGorusme).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Kisi Görüşme Kaydı Güncellenmiştir.");
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
                        m = new Mesaj("hata", "Kisi Görüşme kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
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

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/KisiGorusme/Kisi?kisiID=" + kisiID.ToString() + "&kisiSec=" + kisiSec.ToString(), false);
            return Content("OK");

        }

        private KisiGorusmeVM KisiYeniToEski(KisiGorusmeVM eskiKisi, KisiGorusmeVM yeniKisi)
        {
            if (yeniKisi.kisiGorusme.KisiID != 0)
            {
                eskiKisi.kunye.kunyeID.KisiID = yeniKisi.kisiGorusme.KisiID;
            }

            eskiKisi.yeniGorusme = yeniKisi.yeniGorusme;
            eskiKisi.kisiGorusme.Aciklama = yeniKisi.kisiGorusme.Aciklama;
            eskiKisi.kisiGorusme.AsiDurumu = yeniKisi.kisiGorusme.AsiDurumu;
            eskiKisi.kisiGorusme.BorcBakkal = yeniKisi.kisiGorusme.BorcBakkal;
            eskiKisi.kisiGorusme.BorcDiger = yeniKisi.kisiGorusme.BorcDiger;
            eskiKisi.kisiGorusme.BorcElektrik = yeniKisi.kisiGorusme.BorcElektrik;
            eskiKisi.kisiGorusme.BorcGaz = yeniKisi.kisiGorusme.BorcGaz;
            eskiKisi.kisiGorusme.BorcInternet = yeniKisi.kisiGorusme.BorcInternet;
            eskiKisi.kisiGorusme.BorcTelefon = yeniKisi.kisiGorusme.BorcTelefon;
            eskiKisi.kisiGorusme.CalismaDurumu = yeniKisi.kisiGorusme.CalismaDurumu;
            eskiKisi.kisiGorusme.EgitimDurumu = yeniKisi.kisiGorusme.EgitimDurumu;
            eskiKisi.kisiGorusme.EkBilgi = yeniKisi.kisiGorusme.EkBilgi;
            eskiKisi.kisiGorusme.GorusmeTarihi = yeniKisi.kisiGorusme.GorusmeTarihi;
            eskiKisi.kisiGorusme.id = yeniKisi.kisiGorusme.id;
            eskiKisi.kisiGorusme.KisiID = yeniKisi.kunye.kunyeID.KisiID;
            eskiKisi.kisiGorusme.KronikRahatsizlik = yeniKisi.kisiGorusme.KronikRahatsizlik;
            eskiKisi.kisiGorusme.KronikYuzdesi = yeniKisi.kisiGorusme.KronikYuzdesi;
            eskiKisi.kisiGorusme.MedeniDurumu = yeniKisi.kisiGorusme.MedeniDurumu;
            eskiKisi.kisiGorusme.Meslegi = yeniKisi.kisiGorusme.Meslegi;
            eskiKisi.kisiGorusme.OkulDurumu = yeniKisi.kisiGorusme.OkulDurumu;
            eskiKisi.kisiGorusme.OkurYazar = yeniKisi.kisiGorusme.OkurYazar;
            eskiKisi.kisiGorusme.OzelDurum = yeniKisi.kisiGorusme.OzelDurum;
            eskiKisi.kisiGorusme.SaglikSorunu = yeniKisi.kisiGorusme.SaglikSorunu;
            eskiKisi.kisiGorusme.SosyalDestek = yeniKisi.kisiGorusme.SosyalDestek;
            eskiKisi.kisiGorusme.SosyalGuvencesi = yeniKisi.kisiGorusme.SosyalGuvencesi;
            eskiKisi.kisiGorusme.YonlendirmeDurumu = yeniKisi.kisiGorusme.YonlendirmeDurumu;

            return eskiKisi;
        }

        [ValidateAntiForgeryToken]
        public ActionResult GorusmeSil(long idSil)
        {
            int kisiSec = 0;
            long kisiID = 0;
            try
            {
                kisiSec = (int)Session["kisiSec"];
                kisiID = (long)Session["kisiID"];
            }
            catch { }

            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KISIGORUSME kisi = context.tblKisiGorusme.Find(id);
            context.Entry(kisi).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Kisi Görüşme Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Kisi Görüşme Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/KisiGorusme/Kisi?kisiID=" + kisiID.ToString() + "&kisiSec=" + kisiSec.ToString(), false);

            return Content("OK");
        }

    }
}