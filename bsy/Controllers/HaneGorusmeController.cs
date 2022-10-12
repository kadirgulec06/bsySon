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
                         join ic in context.tblIlceler on mh.IlceID equals ic.id
                         join sy in context.tblSozluk on mh.IlceID equals sy.id
                         join sh in context.tblSehirler on ic.SehirID equals sh.id
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
                         join ic in context.tblIlceler on mh.IlceID equals ic.id
                         join sy in context.tblSozluk on mh.IlceID equals sy.id
                         join sh in context.tblSehirler on ic.SehirID equals sh.id
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

            Column hane = null;
            try
            {
                hane = context.tblHaneGorusme.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (hane == null)
            {
                hane = new Column();
            }

            HaneGorusmeVM haneVM = haneGorusmeHazirla(hane, yeniGorusme);

            return View(haneVM);
        }

        private HaneGorusmeVM haneGorusmeHazirla(Column hane, int yeniGorusme)
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
            hgVM.haneListeleri.IkametYeri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ikametYeriKodu, hgVM.haneGorusme.IkametYeri, 2);
            hgVM.haneListeleri.GocSehri = SozlukHelper.sozlukKalemleriDD(context, SozlukHelper.gocSehriKodu, hgVM.haneGorusme.GocSehri, 2);
            hgVM.haneListeleri.GocIlcesi = SozlukHelper.sozlukKalemleriDD(context, SozlukHelper.gocIlcesiKodu, hgVM.haneGorusme.GocIlcesi, 2);
            hgVM.haneListeleri.GocSebebi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.gocIlcesiKodu, hgVM.haneGorusme.GocSebebi, 2);
            hgVM.haneListeleri.KonusulanDil = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.konusulanDilKodu, hgVM.haneGorusme.KonusulanDil, 2);
            hgVM.haneListeleri.Calisanlar = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.haneCalisanlariKodu, hgVM.haneGorusme.Calisanlar, 2);
            hgVM.haneListeleri.SosyalDestekTuru = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sosyalDestekKodu, hgVM.haneGorusme.SosyalDestekTuru, 2);
            hgVM.haneListeleri.KonutMulkiyetTuru = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.konutMulkiyetKodu, hgVM.haneGorusme.KonutMulkiyetTuru, 2);
            hgVM.haneListeleri.ElektrikErisimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.elektrikErisimiKodu, hgVM.haneGorusme.ElektrikErisimi, 2);
            hgVM.haneListeleri.TemizSu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.temizSuKodu, hgVM.haneGorusme.TemizSu, 2);
            hgVM.haneListeleri.SehirSuyu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sehirSuyuKodu, hgVM.haneGorusme.SehirSuyu, 2);
            hgVM.haneListeleri.Kanalizasyon = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.kanalizasyonGideriKodu, hgVM.haneGorusme.Kanalizasyon, 2);
            hgVM.haneListeleri.BuzDolabi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.buzdolabiKodu, hgVM.haneGorusme.Buzdolabi, 2);
            hgVM.haneListeleri.CamasirMakinesi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.camasirMakinesiKodu, hgVM.haneGorusme.CamasirMakinesi, 2);
            hgVM.haneListeleri.BulasikMakinesi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.bulasikMakinesiKodu, hgVM.haneGorusme.BulasikMakinesi, 2);
            hgVM.haneListeleri.Televizyon = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.televizyonKodu, hgVM.haneGorusme.Televizyon, 2);
            hgVM.haneListeleri.Internet = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.internetImkaniKodu, hgVM.haneGorusme.Internet, 2);
            hgVM.haneListeleri.BilgisayarTablet = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.bilgisayarTabletKodu, hgVM.haneGorusme.BilgisayarTablet, 2);
            hgVM.haneListeleri.Mobilya = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.mobilyaKodu, hgVM.haneGorusme.Mobilya, 2);
            hgVM.haneListeleri.Firin = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.firinKodu, hgVM.haneGorusme.Firin, 2);
            hgVM.haneListeleri.IsinmaTuru = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.isinmaTuruKodu, hgVM.haneGorusme.IsinmaTuru, 2);
            hgVM.haneListeleri.BeslenmeDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.beslenmeDurumuKodu, hgVM.haneGorusme.BeslenmeDurumu, 2);
            hgVM.haneListeleri.BeslenmeIstekleri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.beslenmeIstekleriKodu, hgVM.haneGorusme.BeslenmeIstekleri, 2);
            hgVM.haneListeleri.CocukSutu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.cocukSutuKodu, hgVM.haneGorusme.CocukSutu, 2);
            hgVM.haneListeleri.OgunAtlama = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ogunAtlamaKodu, hgVM.haneGorusme.OgunAtlama, 2);
            hgVM.haneListeleri.OkulYemegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okulYemegiKodu, hgVM.haneGorusme.OkulYemegi, 2);
            hgVM.haneListeleri.OkulBeslenmesi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okulBeslenmesiKodu, hgVM.haneGorusme.OkulBeslenmesi, 2);
            hgVM.haneListeleri.OkulBeslenmeIstekleri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.okulBeslenmeIstekleriKodu, hgVM.haneGorusme.OkulBeslenmeIstekleri, 2);
            hgVM.haneListeleri.AlisVerisYeri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.alisVerisYeriKodu, hgVM.haneGorusme.AlisVerisYeri, 2);
            hgVM.haneListeleri.VeresiyeAlisveris = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.veresiyeAlisVerisKodu, hgVM.haneGorusme.VeresiyeAlisVeris, 2);
            hgVM.haneListeleri.BezMama = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.bezMamaKodu, hgVM.haneGorusme.BezMama, 2);
            hgVM.haneListeleri.OzelIhtiyaclar = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ozelIhtiyaclarKodu, hgVM.haneGorusme.OzelIhtiyaclar, 2);
            hgVM.haneListeleri.KadininMalVarligi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.kadininMalVarligiKodu, hgVM.haneGorusme.KadininMalVarligi, 2);
            hgVM.haneListeleri.CocuklarEgitimBitirme = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.cocuklarEgitimBitirmeKodu, hgVM.haneGorusme.CocuklarEgitimBitirme, 2);
            hgVM.haneListeleri.EgitimEngelleri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.egitimEngelleriKodu, hgVM.haneGorusme.EgitimEngelleri, 2);
            hgVM.haneListeleri.EgitimEngeliCozumleri = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.egitimEngeliCozumleriKodu, hgVM.haneGorusme.EgitimEngeliCozumleri, 2);
            hgVM.haneListeleri.SorunDestegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.sorunDestegiKodu, hgVM.haneGorusme.SorunDestegi, 2);
            hgVM.haneListeleri.CevredeUniversiteli = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.cevredeUniversiteliKodu, hgVM.haneGorusme.CevredeUniversiteli, 2);
            hgVM.haneListeleri.CocukVakitGecirme = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.cocukVakitGecirmeKodu, hgVM.haneGorusme.CocukVakitGecirme, 2);
            hgVM.haneListeleri.AileVakitGecirme = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.aileVakitGecirmeKodu, hgVM.haneGorusme.AileVakitGecirme, 2);
            hgVM.haneListeleri.DestekAlmaPaylasma = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.destekAlmaPaylasmaKodu, hgVM.haneGorusme.DestekAlmaPaylasma, 2);
            hgVM.haneListeleri.CevreGuvenlimi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.cevreGuvenlimiKodu, hgVM.haneGorusme.CevreGuvenlimi, 2);
            hgVM.haneListeleri.FaturaDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.faturaDurumKodu, hgVM.haneGorusme.FaturaDurumu, 2);
            hgVM.haneListeleri.KisiDestegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.kisiDestegiKodu, hgVM.haneGorusme.KisiDestegi, 2);
            hgVM.haneListeleri.KurumDestegi = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.kurumDestegiKodu, hgVM.haneGorusme.KurumDestegi, 2);
            hgVM.haneListeleri.YonlendirmeDurumu = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.yonlendirmeDurumuKodu, hgVM.haneGorusme.YonlendirmeDurumu, 2);
            hgVM.haneListeleri.OzelDurum = SozlukHelper.anaSozlukKalemleriDD(context, SozlukHelper.ozelDurumKodu, hgVM.haneGorusme.OzelDurum, 2);

            return hgVM;
        }

        private Column HaneToHane(Column eskiHG)
        {
            Column yeniHG = new Column();

            yeniHG.Aciklama = eskiHG.Aciklama;
            yeniHG.AileVakitGecirme = eskiHG.AileVakitGecirme;
            yeniHG.AlisVerisYeri = eskiHG.AlisVerisYeri;
            yeniHG.AltmisBesUstuVar = eskiHG.AltmisBesUstuVar;
            yeniHG.BebekGideri = eskiHG.BebekGideri;
            yeniHG.BeslenmeDurumu = eskiHG.BeslenmeDurumu;
            yeniHG.BeslenmeIstekleri = eskiHG.BeslenmeIstekleri;
            yeniHG.BezMama = eskiHG.BezMama;
            yeniHG.BilgisayarTablet = eskiHG.BilgisayarTablet;
            yeniHG.BorcBakkal = eskiHG.BorcBakkal;
            yeniHG.BorcDiger = eskiHG.BorcDiger;
            yeniHG.BorcElektrik = eskiHG.BorcElektrik;
            yeniHG.BorcGaz = eskiHG.BorcGaz;
            yeniHG.BorcInternet = eskiHG.BorcInternet;
            yeniHG.BorcKira = eskiHG.BorcKira;
            yeniHG.BorcKredi = eskiHG.BorcKredi;
            yeniHG.BorcSu = eskiHG.BorcSu;
            yeniHG.BorcTelefon = eskiHG.BorcTelefon;
            yeniHG.BulasikMakinesi = eskiHG.BulasikMakinesi;
            yeniHG.Buzdolabi = eskiHG.Buzdolabi;
            yeniHG.CalisanCocukSayisi = eskiHG.CalisanCocukSayisi;
            yeniHG.CalisanGeliri = eskiHG.CalisanGeliri;
            yeniHG.Calisanlar = eskiHG.Calisanlar;
            yeniHG.CalisanSayisi = eskiHG.CalisanSayisi;
            yeniHG.CamasirMakinesi = eskiHG.CamasirMakinesi;
            yeniHG.CevredeUniversiteli = eskiHG.CevredeUniversiteli;
            yeniHG.CevreGuvenlimi = eskiHG.CevreGuvenlimi;
            yeniHG.CocuklarEgitimBitirme = eskiHG.CocuklarEgitimBitirme;
            yeniHG.CocukOlumuVar = eskiHG.CocukOlumuVar;
            yeniHG.CocukSayisi = eskiHG.CocukSayisi;
            yeniHG.CocukSutu = eskiHG.CocukSutu;
            yeniHG.CocukVakitGecirme = eskiHG.CocukVakitGecirme;
            yeniHG.DestekAlmaPaylasma = eskiHG.DestekAlmaPaylasma;
            yeniHG.DigerGiderler = eskiHG.DigerGiderler;
            yeniHG.EgitimEngeliCozumleri = eskiHG.EgitimEngeliCozumleri;
            yeniHG.EgitimEngelleri = eskiHG.EgitimEngelleri;
            yeniHG.EkBilgi = eskiHG.EkBilgi;
            yeniHG.ElektrikErisimi = eskiHG.ElektrikErisimi;
            yeniHG.ElliAltiOlumVar = eskiHG.ElliAltiOlumVar;
            yeniHG.EmekliMaasi = eskiHG.EmekliMaasi;
            yeniHG.EngelliVar = eskiHG.EngelliVar;
            yeniHG.EvdeYasanilanSure = eskiHG.EvdeYasanilanSure;
            yeniHG.EvdeYasayanlar = eskiHG.EvdeYasayanlar;
            yeniHG.FaturaDurumu = eskiHG.FaturaDurumu;
            yeniHG.Faturalar = eskiHG.Faturalar;
            yeniHG.Firin = eskiHG.Firin;
            yeniHG.GidaMasraflari = eskiHG.GidaMasraflari;
            yeniHG.GocIlcesi = eskiHG.GocIlcesi;
            yeniHG.GocSebebi = eskiHG.GocSebebi;
            yeniHG.GocSehri = eskiHG.GocSehri;
            yeniHG.GocYili = eskiHG.GocYili;
            yeniHG.GorusmeTarihi = eskiHG.GorusmeTarihi;
            yeniHG.HaneID = eskiHG.HaneID;
            yeniHG.id = 0;  // eskiHG.id;
            yeniHG.IkametYeri = eskiHG.IkametYeri;
            yeniHG.Internet = eskiHG.Internet;
            yeniHG.IsinmaGideri = eskiHG.IsinmaGideri;
            yeniHG.IsinmaTuru = eskiHG.IsinmaTuru;
            yeniHG.KadininMalVarligi = eskiHG.KadininMalVarligi;
            yeniHG.Kanalizasyon = eskiHG.Kanalizasyon;
            yeniHG.KiraTutari = eskiHG.KiraTutari;
            yeniHG.KisiDestegi = eskiHG.KisiDestegi;
            yeniHG.KonusulanDil = eskiHG.KonusulanDil;
            yeniHG.KonutMulkiyetTuru = eskiHG.KonutMulkiyetTuru;
            yeniHG.KronikEngelliSayisi = eskiHG.KronikEngelliSayisi;
            yeniHG.KronikVar = eskiHG.KronikVar;
            yeniHG.KurumDestegi = eskiHG.KurumDestegi;
            yeniHG.Mobilya = eskiHG.Mobilya;
            yeniHG.OgunAtlama = eskiHG.OgunAtlama;
            yeniHG.OkulBeslenmeIstekleri = eskiHG.OkulBeslenmeIstekleri;
            yeniHG.OkulBeslenmesi = eskiHG.OkulBeslenmesi;
            yeniHG.OkulYemegi = eskiHG.OkulYemegi;
            yeniHG.OrtalamaGelir = eskiHG.OrtalamaGelir;
            yeniHG.OzelDurum = eskiHG.OzelDurum;
            yeniHG.OzelIhtiyaclar = eskiHG.OzelIhtiyaclar;
            yeniHG.SehirSuyu = eskiHG.SehirSuyu;
            yeniHG.SorunDestegi = eskiHG.SorunDestegi;
            yeniHG.SosyalDestekTuru = eskiHG.SosyalDestekTuru;
            yeniHG.Taksitler = eskiHG.Taksitler;
            yeniHG.Televizyon = eskiHG.Televizyon;
            yeniHG.TemizSu = eskiHG.TemizSu;
            yeniHG.VeresiyeAlisVeris = eskiHG.VeresiyeAlisVeris;
            yeniHG.YetiskinSayisi = eskiHG.YetiskinSayisi;
            yeniHG.YonlendirmeDurumu = eskiHG.YonlendirmeDurumu;

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

            Column hane = context.tblHaneGorusme.Find(yeniHane.haneGorusme.id);
            if (hane == null)
            {
                hane = new Column();
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

            eskiHane.haneGorusme.Aciklama = yeniHane.haneGorusme.Aciklama;
            eskiHane.haneGorusme.AileVakitGecirme = yeniHane.haneGorusme.AileVakitGecirme;
            eskiHane.haneGorusme.AlisVerisYeri = yeniHane.haneGorusme.AlisVerisYeri;
            eskiHane.haneGorusme.AltmisBesUstuVar = yeniHane.haneGorusme.AltmisBesUstuVar;
            eskiHane.haneGorusme.BebekGideri = yeniHane.haneGorusme.BebekGideri;
            eskiHane.haneGorusme.BeslenmeDurumu = yeniHane.haneGorusme.BeslenmeDurumu;
            eskiHane.haneGorusme.BeslenmeIstekleri = yeniHane.haneGorusme.BeslenmeIstekleri;
            eskiHane.haneGorusme.BezMama = yeniHane.haneGorusme.BezMama;
            eskiHane.haneGorusme.BilgisayarTablet = yeniHane.haneGorusme.BilgisayarTablet;
            eskiHane.haneGorusme.BorcBakkal = yeniHane.haneGorusme.BorcBakkal;
            eskiHane.haneGorusme.BorcDiger = yeniHane.haneGorusme.BorcDiger;
            eskiHane.haneGorusme.BorcElektrik = yeniHane.haneGorusme.BorcElektrik;
            eskiHane.haneGorusme.BorcGaz = yeniHane.haneGorusme.BorcGaz;
            eskiHane.haneGorusme.BorcInternet = yeniHane.haneGorusme.BorcInternet;
            eskiHane.haneGorusme.BorcKira = yeniHane.haneGorusme.BorcKira;
            eskiHane.haneGorusme.BorcKredi = yeniHane.haneGorusme.BorcKredi;
            eskiHane.haneGorusme.BorcSu = yeniHane.haneGorusme.BorcSu;
            eskiHane.haneGorusme.BorcTelefon = yeniHane.haneGorusme.BorcTelefon;
            eskiHane.haneGorusme.BulasikMakinesi = yeniHane.haneGorusme.BulasikMakinesi;
            eskiHane.haneGorusme.Buzdolabi = yeniHane.haneGorusme.Buzdolabi;
            eskiHane.haneGorusme.CalisanCocukSayisi = yeniHane.haneGorusme.CalisanCocukSayisi;
            eskiHane.haneGorusme.CalisanGeliri = yeniHane.haneGorusme.CalisanGeliri;
            eskiHane.haneGorusme.Calisanlar = yeniHane.haneGorusme.Calisanlar;
            eskiHane.haneGorusme.CalisanSayisi = yeniHane.haneGorusme.CalisanSayisi;
            eskiHane.haneGorusme.CamasirMakinesi = yeniHane.haneGorusme.CamasirMakinesi;
            eskiHane.haneGorusme.CevredeUniversiteli = yeniHane.haneGorusme.CevredeUniversiteli;
            eskiHane.haneGorusme.CevreGuvenlimi = yeniHane.haneGorusme.CevreGuvenlimi;
            eskiHane.haneGorusme.CocuklarEgitimBitirme = yeniHane.haneGorusme.CocuklarEgitimBitirme;
            eskiHane.haneGorusme.CocukOlumuVar = yeniHane.haneGorusme.CocukOlumuVar;
            eskiHane.haneGorusme.CocukSayisi = yeniHane.haneGorusme.CocukSayisi;
            eskiHane.haneGorusme.CocukSutu = yeniHane.haneGorusme.CocukSutu;
            eskiHane.haneGorusme.CocukVakitGecirme = yeniHane.haneGorusme.CocukVakitGecirme;
            eskiHane.haneGorusme.DestekAlmaPaylasma = yeniHane.haneGorusme.DestekAlmaPaylasma;
            eskiHane.haneGorusme.DigerGiderler = yeniHane.haneGorusme.DigerGiderler;
            eskiHane.haneGorusme.EgitimEngeliCozumleri = yeniHane.haneGorusme.EgitimEngeliCozumleri;
            eskiHane.haneGorusme.EgitimEngelleri = yeniHane.haneGorusme.EgitimEngelleri;
            eskiHane.haneGorusme.EkBilgi = yeniHane.haneGorusme.EkBilgi;
            eskiHane.haneGorusme.ElektrikErisimi = yeniHane.haneGorusme.ElektrikErisimi;
            eskiHane.haneGorusme.ElliAltiOlumVar = yeniHane.haneGorusme.ElliAltiOlumVar;
            eskiHane.haneGorusme.EmekliMaasi = yeniHane.haneGorusme.EmekliMaasi;
            eskiHane.haneGorusme.EngelliVar = yeniHane.haneGorusme.EngelliVar;
            eskiHane.haneGorusme.EvdeYasanilanSure = yeniHane.haneGorusme.EvdeYasanilanSure;
            eskiHane.haneGorusme.EvdeYasayanlar = yeniHane.haneGorusme.EvdeYasayanlar;
            eskiHane.haneGorusme.FaturaDurumu = yeniHane.haneGorusme.FaturaDurumu;
            eskiHane.haneGorusme.Faturalar = yeniHane.haneGorusme.Faturalar;
            eskiHane.haneGorusme.Firin = yeniHane.haneGorusme.Firin;
            eskiHane.haneGorusme.GidaMasraflari = yeniHane.haneGorusme.GidaMasraflari;
            eskiHane.haneGorusme.GocIlcesi = yeniHane.haneGorusme.GocIlcesi;
            eskiHane.haneGorusme.GocSebebi = yeniHane.haneGorusme.GocSebebi;
            eskiHane.haneGorusme.GocSehri = yeniHane.haneGorusme.GocSehri;
            eskiHane.haneGorusme.GocYili = yeniHane.haneGorusme.GocYili;
            eskiHane.haneGorusme.GorusmeTarihi = yeniHane.haneGorusme.GorusmeTarihi;
            //eskiHane.haneGorusme.HaneID = yeniHane.haneGorusme.HaneID;
            eskiHane.haneGorusme.HaneID = yeniHane.kunye.kunyeID.HaneID;
            eskiHane.haneGorusme.id = yeniHane.haneGorusme.id;
            eskiHane.haneGorusme.IkametYeri = yeniHane.haneGorusme.IkametYeri;
            eskiHane.haneGorusme.Internet = yeniHane.haneGorusme.Internet;
            eskiHane.haneGorusme.IsinmaGideri = yeniHane.haneGorusme.IsinmaGideri;
            eskiHane.haneGorusme.IsinmaTuru = yeniHane.haneGorusme.IsinmaTuru;
            eskiHane.haneGorusme.KadininMalVarligi = yeniHane.haneGorusme.KadininMalVarligi;
            eskiHane.haneGorusme.Kanalizasyon = yeniHane.haneGorusme.Kanalizasyon;
            eskiHane.haneGorusme.KiraTutari = yeniHane.haneGorusme.KiraTutari;
            eskiHane.haneGorusme.KisiDestegi = yeniHane.haneGorusme.KisiDestegi;
            eskiHane.haneGorusme.KonusulanDil = yeniHane.haneGorusme.KonusulanDil;
            eskiHane.haneGorusme.KonutMulkiyetTuru = yeniHane.haneGorusme.KonutMulkiyetTuru;
            eskiHane.haneGorusme.KronikEngelliSayisi = yeniHane.haneGorusme.KronikEngelliSayisi;
            eskiHane.haneGorusme.KronikVar = yeniHane.haneGorusme.KronikVar;
            eskiHane.haneGorusme.KurumDestegi = yeniHane.haneGorusme.KurumDestegi;
            eskiHane.haneGorusme.Mobilya = yeniHane.haneGorusme.Mobilya;
            eskiHane.haneGorusme.OgunAtlama = yeniHane.haneGorusme.OgunAtlama;
            eskiHane.haneGorusme.OkulBeslenmeIstekleri = yeniHane.haneGorusme.OkulBeslenmeIstekleri;
            eskiHane.haneGorusme.OkulBeslenmesi = yeniHane.haneGorusme.OkulBeslenmesi;
            eskiHane.haneGorusme.OkulYemegi = yeniHane.haneGorusme.OkulYemegi;
            eskiHane.haneGorusme.OrtalamaGelir = yeniHane.haneGorusme.OrtalamaGelir;
            eskiHane.haneGorusme.OzelDurum = yeniHane.haneGorusme.OzelDurum;
            eskiHane.haneGorusme.OzelIhtiyaclar = yeniHane.haneGorusme.OzelIhtiyaclar;
            eskiHane.haneGorusme.SehirSuyu = yeniHane.haneGorusme.SehirSuyu;
            eskiHane.haneGorusme.SorunDestegi = yeniHane.haneGorusme.SorunDestegi;
            eskiHane.haneGorusme.SosyalDestekTuru = yeniHane.haneGorusme.SosyalDestekTuru;
            eskiHane.haneGorusme.Taksitler = yeniHane.haneGorusme.Taksitler;
            eskiHane.haneGorusme.Televizyon = yeniHane.haneGorusme.Televizyon;
            eskiHane.haneGorusme.TemizSu = yeniHane.haneGorusme.TemizSu;
            eskiHane.haneGorusme.VeresiyeAlisVeris = yeniHane.haneGorusme.VeresiyeAlisVeris;
            eskiHane.haneGorusme.YetiskinSayisi = yeniHane.haneGorusme.YetiskinSayisi;
            eskiHane.haneGorusme.YonlendirmeDurumu = yeniHane.haneGorusme.YonlendirmeDurumu;


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

            Column hane = context.tblHaneGorusme.Find(id);
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