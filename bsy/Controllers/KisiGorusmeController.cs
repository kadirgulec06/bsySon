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

            string sehirIlceMahalle = "";
            string haneKodu = "";
            string adres = "";
            string adSoyad = "";
            string tcNo = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["SEHIRILCEMAHALLE"] != null)
                {
                    sehirIlceMahalle = Request.Params["SEHIRILCEMAHALLE"];
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
                Session["filtreSEHIRILCEMAHALLE"] = sehirIlceMahalle.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
                Session["filtreADRES"] = adres.ToUpper();
                Session["filtreADSOYAD"] = adSoyad.ToUpper();
                Session["filtreTCNO"] = tcNo.ToUpper();
            }
            else if (rapor == 1)
            {
                sehirIlceMahalle = (string)Session["filtreSEHIRILCEMAHALLE"];
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
                                      (kh.KisiID == kisiID || kisiID == 0) &&
                                      (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                                    group kh by kh.KisiID into khGRP
                                    select new { KisiID = khGRP.Key, Tarih = khGRP.Max(x => x.BasTar) });

            var sonKisiHane = (from kht in sonKisiHaneTarih
                               join kh in context.tblKisiHane on new { x1 = kht.KisiID, x2 = kht.Tarih } equals new { x1 = kh.KisiID, x2 = kh.BasTar }
                               select kh);
            */

            var query = (from kx in context.tblKisiler
                         join kh in context.tblKisiHane on kx.id equals kh.KisiID
                         join hn in context.tblHaneler on kh.HaneID equals hn.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sm in context.tblSozluk on mh.id equals sm.id
                         join ic in context.tblIlceler on mh.IlceID equals ic.id
                         join sy in context.tblSozluk on mh.IlceID equals sy.id
                         join sh in context.tblSehirler on ic.SehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id)) &&
                            (sy.Aciklama + " " + sz.Aciklama + " " + sm.Aciklama + "").Contains(sehirIlceMahalle) &&
                            (hn.HaneKodu + "").Contains(haneKodu) &&
                            (hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + " " + hn.Daire + "").Contains(adres) &&
                            (kx.Ad + " " + kx.Soyad + "").Contains(adSoyad) &&
                            (kx.TCNo + "").Contains(tcNo)
                         select new
                         {
                             kx.id,
                             hn.HaneKodu,
                             sehirIlceMahalle = sz.Aciklama + "-" + sy.Aciklama + sm.Aciklama,
                             mahalleADI = sm.Aciklama,
                             Adres = hn.Cadde + " "+ hn.Sokak + " " + hn.Apartman + " " + hn.Daire,
                             AdSoyad = kx.Ad + " " + kx.Soyad,
                             kx.TCNo
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("AdSoyad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kx in resultSetAfterOrderandPaging
                             select new
                             {
                                 kx.id,
                                 kx.HaneKodu,
                                 kx.sehirIlceMahalle,
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
                                 kx.sehirIlceMahalle,
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

            string sehirIlceMahalle = "";
            string adres = "";
            string haneKodu = "";
            string adSoyad = "";
            string tcNo = "";

            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["SEHIRILCE"] != null)
                {
                    sehirIlceMahalle = Request.Params["SEHIRILCE"];
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
                Session["filtreSEHIRILCEMAHALLE"] = sehirIlceMahalle.ToUpper();
                Session["filtreADRES"] = adres.ToUpper();
                Session["filtreHANEKODU"] = haneKodu.ToUpper();
                Session["filtreADSOYAD"] = adSoyad.ToUpper();
                Session["filtreTCNO"] = tcNo.ToUpper();
            }
            else if (rapor == 1)
            {
                sehirIlceMahalle = (string)Session["filtreSEHIRILCEMAHALLE"];
                adres = (string)Session["filtreADRES"];
                haneKodu = (string)Session["filtreHANEKODU"];
                adSoyad = (string)Session["filtreADSOYAD"];
                tcNo = (string)Session["filtreTCNO"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            DateTime bugun = DateTime.Now.Date;

            /*
            var sonKH = (from kh in context.tblKisiHane
                         join hn in context.tblHaneler on kh.HaneID equals hn.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         where
                           kh.BitTar > bugun &&
                           (kh.KisiID == kisiID || kisiID == 0) &&
                           (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                         group kh by kh.KisiID into khGRP
                         let maxBastar = khGRP.Max(khx => khx.BasTar)

                         from kh in khGRP
                         where kh.BasTar == maxBastar
                         select kh).ToList();

            var sonKisiHaneTarih = (from kh in context.tblKisiHane
                                    join hn in context.tblHaneler on kh.HaneID equals hn.id
                                    join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                                    where
                                      kh.BitTar > bugun &&
                                      (kh.KisiID == kisiID || kisiID == 0) &&
                                      (user.gy.butunTurkiye == true || user.gy.mahalleler.Contains(mh.id))
                                    group kh by kh.KisiID into khGRP
                                    select new { KisiID = khGRP.Key, Tarih = khGRP.Max(x => x.BasTar) });

            var sonKisiHane = (from kht in sonKisiHaneTarih
                               join kh in context.tblKisiHane on new { x1 = kht.KisiID, x2 = kht.Tarih } equals new { x1 = kh.KisiID, x2 = kh.BasTar }
                               select kh);
            */

            //pageSize = 5;
            var query = (from kg in context.tblKisiGorusme
                         join kx in context.tblKisiler on kg.KisiID equals kx.id
                         join kh in context.tblKisiHane on kx.id equals kh.KisiID
                         join hn in context.tblHaneler on kh.HaneID equals hn.id
                         join mh in context.tblMahalleler on hn.MahalleID equals mh.id
                         join sx in context.tblSozluk on mh.id equals sx.id
                         join ic in context.tblIlceler on mh.IlceID equals ic.id
                         join sy in context.tblSozluk on mh.IlceID equals sy.id
                         join sh in context.tblSehirler on ic.SehirID equals sh.id
                         join sz in context.tblSozluk on sh.id equals sz.id
                         where
                            (kg.KisiID == kisiID || kisiID == 0) &&
                            //(kg.GorusmeTarihi >= kh.BasTar && kg.GorusmeTarihi <= kh.BitTar) &&
                            (user.gy.butunTurkiye == true ||
                            user.gy.mahalleler.Contains(hn.MahalleID)) &&
                            (sz.Aciklama + " " + sy.Aciklama + " " + sx.Aciklama + "").Contains(sehirIlceMahalle) &&
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
                             sehirIlceMahalle = sz.Aciklama + "-" + sy.Aciklama + sx.Aciklama,
                             Adres = hn.Cadde + " " + hn.Sokak + " " + hn.Apartman + "-" + hn.Daire,
                             AdSoyad = kx.Ad + " " + kx.Soyad,
                             kg.GorusmeTarihi,
                             kg.Aciklama
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("HaneKodu").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kg in resultSetAfterOrderandPaging
                             select new
                             {
                                 kg.id,
                                 kg.kisiID,
                                 kg.haneID,
                                 kg.HaneKodu,
                                 kg.sehirIlceMahalle,
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
                                 kg.HaneKodu,
                                 kg.sehirIlceMahalle,
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
            kgVM.kisiListeleri.MedeniDurum = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.medeniDurumKodu, kgVM.kisiGorusme.MedeniDurum, 2);
            kgVM.kisiListeleri.OkurYazar = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okurYazarKodu, kgVM.kisiGorusme.OkurYazar, 2);
            kgVM.kisiListeleri.EgitimDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.egitimDurumuKodu, kgVM.kisiGorusme.EgitimDurumu, 2);
            kgVM.kisiListeleri.EgitimSon = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.egitimSonKodu, kgVM.kisiGorusme.EgitimSon, 2);
            kgVM.kisiListeleri.OkulDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ozelDurumKodu, kgVM.kisiGorusme.OkulDurumu, 2);
            kgVM.kisiListeleri.Sinif = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sinifKodu, kgVM.kisiGorusme.Sinif, 2);
            kgVM.kisiListeleri.OkulaDevam = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okulaDevamKodu, kgVM.kisiGorusme.OkulaDevam, 2);
            kgVM.kisiListeleri.SinifTekrari = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sinifTekrariKodu, kgVM.kisiGorusme.SinifTekrari, 2);
            kgVM.kisiListeleri.CocukOdasi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.cocukOdasiKodu, kgVM.kisiGorusme.CocukOdasi, 2);
            kgVM.kisiListeleri.OdevYardimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.odevYardimiKodu, kgVM.kisiGorusme.OdevYardimi, 2);
            kgVM.kisiListeleri.OkulIstegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okulIstegiKodu, kgVM.kisiGorusme.OkulIstegi, 2);
            kgVM.kisiListeleri.OkulServisi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okulServisiKodu, kgVM.kisiGorusme.OkulServisi, 2);
            kgVM.kisiListeleri.DersDestegiIhtiyaci = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.dersDestegiIhtiyaciKodu, kgVM.kisiGorusme.DersDestegiIhtiyaci, 2);
            kgVM.kisiListeleri.CalismaDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.calismaDurumuKodu, kgVM.kisiGorusme.CalismaDurumu, 2);
            kgVM.kisiListeleri.Meslek = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.meslekKodu, kgVM.kisiGorusme.Meslek, 2);
            kgVM.kisiListeleri.Is = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.isKodu, kgVM.kisiGorusme.Is, 2);
            kgVM.kisiListeleri.CalismaIstegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.calismaIstegiKodu, kgVM.kisiGorusme.CalismaIstegi, 2);
            kgVM.kisiListeleri.SosyalGuvence = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sosyalGuvenceKodu, kgVM.kisiGorusme.SosyalGuvence, 2);
            kgVM.kisiListeleri.SaglikSigortasi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.saglikSigortasiKodu, kgVM.kisiGorusme.SaglikSigortasi, 2);
            kgVM.kisiListeleri.SaglikDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.saglikDurumuKodu, kgVM.kisiGorusme.SaglikDurumu, 2);
            kgVM.kisiListeleri.KronikDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.kronikDurumuKodu, kgVM.kisiGorusme.KronikDurumu, 2);
            kgVM.kisiListeleri.DuzenliIlacIhtiyaci = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.duzenliIlacIhtiyaciKodu, kgVM.kisiGorusme.DuzenliIlacIhtiyaci, 2);
            kgVM.kisiListeleri.IlacTeminDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ilacTeminDurumuKodu, kgVM.kisiGorusme.IlacTeminDurumu, 2);
            kgVM.kisiListeleri.PsikolojikDestekIhtiyaci = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.psikolojikDestekCozumuKodu, kgVM.kisiGorusme.PsikolojikDestekIhtiyaci, 2);
            kgVM.kisiListeleri.PsikolojikDestekCozumleri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.psikolojikDestekCozumuKodu, kgVM.kisiGorusme.PsikolojikDestekCozumleri, 2);
            kgVM.kisiListeleri.SagliktaGittigiYer = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sagliktaGittigiYerKodu, kgVM.kisiGorusme.SagliktaGittigiYerler, 2);
            kgVM.kisiListeleri.SaglikYeriSorunlari = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.saglikYeriSorunuKodu, kgVM.kisiGorusme.SaglikYeriSorunlari, 2);
            kgVM.kisiListeleri.DogumKontroluIhtiyaci = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.dogumKontroluIhtiyaciKodu, kgVM.kisiGorusme.DogumKontroluIhtiyaci, 2);
            kgVM.kisiListeleri.DogumKontroluIstegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.dogumKontroluIstegiKodu, kgVM.kisiGorusme.DogumKontroluIstegi, 2);
            kgVM.kisiListeleri.HamilelikKontrolleri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.hamilelikKontrolleriKodu, kgVM.kisiGorusme.HamilelikKontrolleri, 2);
            kgVM.kisiListeleri.AsiDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.asiDurumuKodu, kgVM.kisiGorusme.AsiDurumu, 2);

            return kgVM;
        }

        private KISIGORUSME KisiToKisi(KISIGORUSME eskiKG)
        {
            KISIGORUSME yeniKG = new KISIGORUSME();

            yeniKG.CalismaDurumu = eskiKG.CalismaDurumu;

            yeniKG.Aciklama = eskiKG.Aciklama;
            yeniKG.AsiDurumu = eskiKG.AsiDurumu;
            yeniKG.Boy = eskiKG.Boy;
            yeniKG.CalismaDurumu = eskiKG.CalismaDurumu;
            yeniKG.CalismaIstegi = eskiKG.CalismaIstegi;
            yeniKG.CocukOdasi = eskiKG.CocukOdasi;
            yeniKG.DersDestegiIhtiyaci = eskiKG.DersDestegiIhtiyaci;
            yeniKG.DogumKontroluIhtiyaci = eskiKG.DogumKontroluIhtiyaci;
            yeniKG.DogumKontroluIstegi = eskiKG.DogumKontroluIstegi;
            yeniKG.DuzenliIlacIhtiyaci = eskiKG.DuzenliIlacIhtiyaci;
            yeniKG.EgitimDurumu = eskiKG.EgitimDurumu;
            yeniKG.EgitimSon = eskiKG.EgitimSon;
            yeniKG.EkBilgi = eskiKG.EkBilgi;
            yeniKG.GorusmeTarihi = eskiKG.GorusmeTarihi;
            yeniKG.HamilelikKontrolleri = eskiKG.HamilelikKontrolleri;
            yeniKG.id = 0;
            yeniKG.IlacTeminDurumu = eskiKG.IlacTeminDurumu;
            yeniKG.Is = eskiKG.Is;
            yeniKG.IsGunuHikayesi = eskiKG.IsGunuHikayesi;
            yeniKG.Kilo = eskiKG.Kilo;
            yeniKG.KisiID = eskiKG.KisiID;
            yeniKG.KronikDurumu = eskiKG.KronikDurumu;
            yeniKG.KronikYuzdesi = eskiKG.KronikYuzdesi;
            yeniKG.MedeniDurum = eskiKG.MedeniDurum;
            yeniKG.Meslek = eskiKG.Meslek;
            yeniKG.OdevYardimi = eskiKG.OdevYardimi;
            yeniKG.OkulaDevam = eskiKG.OkulaDevam;
            yeniKG.OkulDurumu = eskiKG.OkulDurumu;
            yeniKG.OkulIstegi = eskiKG.OkulIstegi;
            yeniKG.OkulServisi = eskiKG.OkulServisi;
            yeniKG.OkurYazar = eskiKG.OkurYazar;
            yeniKG.PsikolojikDestekCozumleri = eskiKG.PsikolojikDestekCozumleri;
            yeniKG.PsikolojikDestekIhtiyaci = eskiKG.PsikolojikDestekIhtiyaci;
            yeniKG.SaglikDurumu = eskiKG.SaglikDurumu;
            yeniKG.SaglikSigortasi = eskiKG.SaglikSigortasi;
            yeniKG.SagliktaGittigiYerler = eskiKG.SagliktaGittigiYerler;
            yeniKG.SaglikYeriSorunlari = eskiKG.SaglikYeriSorunlari;
            yeniKG.Sinif = eskiKG.Sinif;
            yeniKG.SinifTekrari = eskiKG.SinifTekrari;
            yeniKG.SosyalGuvence = eskiKG.SosyalGuvence;

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
            eskiKisi.kisiGorusme.Boy = yeniKisi.kisiGorusme.Boy;
            eskiKisi.kisiGorusme.CalismaDurumu = yeniKisi.kisiGorusme.CalismaDurumu;
            eskiKisi.kisiGorusme.CalismaIstegi = yeniKisi.kisiGorusme.CalismaIstegi;
            eskiKisi.kisiGorusme.CocukOdasi = yeniKisi.kisiGorusme.CocukOdasi;
            eskiKisi.kisiGorusme.DersDestegiIhtiyaci = yeniKisi.kisiGorusme.DersDestegiIhtiyaci;
            eskiKisi.kisiGorusme.DogumKontroluIhtiyaci = yeniKisi.kisiGorusme.DogumKontroluIhtiyaci;
            eskiKisi.kisiGorusme.DogumKontroluIstegi = yeniKisi.kisiGorusme.DogumKontroluIstegi;
            eskiKisi.kisiGorusme.DuzenliIlacIhtiyaci = yeniKisi.kisiGorusme.DuzenliIlacIhtiyaci;
            eskiKisi.kisiGorusme.EgitimDurumu = yeniKisi.kisiGorusme.EgitimDurumu;
            eskiKisi.kisiGorusme.EgitimSon = yeniKisi.kisiGorusme.EgitimSon;
            eskiKisi.kisiGorusme.EkBilgi = yeniKisi.kisiGorusme.EkBilgi;
            eskiKisi.kisiGorusme.GorusmeTarihi = yeniKisi.kisiGorusme.GorusmeTarihi;
            eskiKisi.kisiGorusme.HamilelikKontrolleri = yeniKisi.kisiGorusme.HamilelikKontrolleri;
            eskiKisi.kisiGorusme.id = yeniKisi.kisiGorusme.id;
            eskiKisi.kisiGorusme.IlacTeminDurumu = yeniKisi.kisiGorusme.IlacTeminDurumu;
            eskiKisi.kisiGorusme.Is = yeniKisi.kisiGorusme.Is;
            eskiKisi.kisiGorusme.IsGunuHikayesi = yeniKisi.kisiGorusme.IsGunuHikayesi;
            eskiKisi.kisiGorusme.Kilo = yeniKisi.kisiGorusme.Kilo;
            //eskiKisi.kisiGorusme.KisiID = yeniKisi.kisiGorusme.KisiID;
            eskiKisi.kisiGorusme.KisiID = yeniKisi.kunye.kunyeID.KisiID;
            eskiKisi.kisiGorusme.KronikDurumu = yeniKisi.kisiGorusme.KronikDurumu;
            eskiKisi.kisiGorusme.KronikYuzdesi = yeniKisi.kisiGorusme.KronikYuzdesi;
            eskiKisi.kisiGorusme.MedeniDurum = yeniKisi.kisiGorusme.MedeniDurum;
            eskiKisi.kisiGorusme.Meslek = yeniKisi.kisiGorusme.Meslek;
            eskiKisi.kisiGorusme.OdevYardimi = yeniKisi.kisiGorusme.OdevYardimi;
            eskiKisi.kisiGorusme.OkulaDevam = yeniKisi.kisiGorusme.OkulaDevam;
            eskiKisi.kisiGorusme.OkulDurumu = yeniKisi.kisiGorusme.OkulDurumu;
            eskiKisi.kisiGorusme.OkulIstegi = yeniKisi.kisiGorusme.OkulIstegi;
            eskiKisi.kisiGorusme.OkulServisi = yeniKisi.kisiGorusme.OkulServisi;
            eskiKisi.kisiGorusme.OkurYazar = yeniKisi.kisiGorusme.OkurYazar;
            eskiKisi.kisiGorusme.PsikolojikDestekCozumleri = yeniKisi.kisiGorusme.PsikolojikDestekCozumleri;
            eskiKisi.kisiGorusme.PsikolojikDestekIhtiyaci = yeniKisi.kisiGorusme.PsikolojikDestekIhtiyaci;
            eskiKisi.kisiGorusme.SaglikDurumu = yeniKisi.kisiGorusme.SaglikDurumu;
            eskiKisi.kisiGorusme.SaglikSigortasi = yeniKisi.kisiGorusme.SaglikSigortasi;
            eskiKisi.kisiGorusme.SagliktaGittigiYerler = yeniKisi.kisiGorusme.SagliktaGittigiYerler;
            eskiKisi.kisiGorusme.SaglikYeriSorunlari = yeniKisi.kisiGorusme.SaglikYeriSorunlari;
            eskiKisi.kisiGorusme.Sinif = yeniKisi.kisiGorusme.Sinif;
            eskiKisi.kisiGorusme.SinifTekrari = yeniKisi.kisiGorusme.SinifTekrari;
            eskiKisi.kisiGorusme.SosyalGuvence = yeniKisi.kisiGorusme.SosyalGuvence;


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