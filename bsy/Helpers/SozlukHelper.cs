using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Helpers
{
    public static class SozlukHelper
    {
        public static string rolTuru = "ROL";
        public static string bolgeKodu = "BOLGE";
        public static string sehirKodu = "SEHIR";
        public static string ilceKodu = "ILCE";
        public static string mahalleKodu = "MAHALLE";
        public static string haneKodu = "HANE";
        public static string kisiKodu = "KISI";

        // Hane Sözlük Kodları

        public static string haneTipiKodu = "HANETIPI";

            public static string gocSehriKodu = "SEHIR";
            public static string gocIlcesiKodu = "ILCE";
            public static string ikametYeriKodu = "IKAMET";
            public static string gocSebebiKodu = "GOCSEBEBI";
            public static string konusulanDilKodu= "DIL";
            public static string haneCalisanlariKodu = "HANECALISAN";
            public static string sosyalDestekKodu = "SOSYALDESTEK";
            public static string konutMulkiyetKodu = "KONUTMULKIYETI";
            public static string elektrikErisimiKodu = "ELEKTRIKERISIMI";
            public static string temizSuKodu = "TEMIZSU";
            public static string sehirSuyuKodu = "SEHIRSUYU";
            public static string kanalizasyonGideriKodu = "KANALIZASYONGIDERI";
            public static string buzdolabiKodu = "BUZDOLABI";
            public static string camasirMakinesiKodu = "CAMASIRMAKINESİ";
            public static string bulasikMakinesiKodu = "BULASIKMAKINESI";
            public static string televizyonKodu = "TELEVIZYON";
            public static string internetImkaniKodu = "INTERNETIMKANI";
            public static string bilgisayarTabletKodu  = "BILGISAYARTABLET";
            public static string mobilyaKodu = "MOBILYA";
            public static string firinKodu = "FIRIN";
            public static string isinmaTuruKodu = "ISINMATURU";
            public static string beslenmeDurumuKodu = "BESLENME";
            public static string beslenmeIstekleriKodu = "BESLENMEISTEGI";
            public static string cocukSutuKodu = "COCUKSUTU";
            public static string ogunAtlamaKodu = "OGUNATLAMA";
            public static string okulYemegiKodu = "OKULYEMEGI";
            public static string okulBeslenmesiKodu = "OKULBESLENMESI";
            public static string okulBeslenmeIstekleriKodu = "OKULBESLENMEISTEGI";
            public static string alisVerisYeriKodu = "ALISVERISYERI";
            public static string veresiyeAlisVerisKodu = "VERESIYEALISVERIS";
            public static string bezMamaKodu = "BEZMAMA";
            public static string ozelIhtiyaclarKodu = "OZELIHTIYAC";
            public static string kadininMalVarligiKodu = "KADINMALVARLIGI";
            public static string cocuklarEgitimBitirmeKodu = "EGITIMBITIRME";
            public static string egitimEngelleriKodu = "EGITIMENGELI";
            public static string egitimEngeliCozumleriKodu = "EGITIMENGELIZCOZUMU";
            public static string sorunDestegiKodu = "SORUNDESTEGI";
            public static string cevredeUniversiteliKodu = "CEVREDEUNIVERSITELI";
            public static string cocukVakitGecirmeKodu = "COCUKVAKITGECİRME";
            public static string aileVakitGecirmeKodu = "AILEVAKITGECIRME";
            public static string destekAlmaPaylasmaKodu = "DESTEKALMAPAYLASMA";
            public static string cevreGuvenlimiKodu = "CEVREGUVENLIGI";
            public static string faturaDurumKodu = "FATURADURUMU";
            public static string kisiDestegiKodu = "KISIDESTEGI";
            public static string kurumDestegiKodu = "KURUMDESTEGI";
            public static string yonlendirmeDurumuKodu = "YONLENDIRME";
            public static string ozelDurumKodu = "OZELDURUM";

        // Kişi Sözlük Kodları

            public static string cinsiyetKodu = "CINSIYET";

            public static string medeniDurumKodu  = "MEDENIDURUM";
            public static string okurYazarKodu = "OKURYAZAR";
            public static string egitimDurumuKodu = "EGITIMDURUMU";
            public static string egitimSonKodu = "EGITIMSON";
            public static string okulDurumuKodu = "OKULDURUMU";
            public static string sinifKodu = "SINIF";
            public static string okulaDevamKodu = "OKULADEVAM";
            public static string sinifTekrariKodu = "SINIFTEKRARI";
            public static string cocukOdasiKodu = "COCUKODASI";
            public static string odevYardimiKodu = "ODEVYARDIMI";
            public static string okulIstegiKodu = "OKULISTEGI";
            public static string okulServisiKodu = "OKULSERVISI";
            public static string dersDestegiIhtiyaciKodu = "DERSDESTEGIIHTIYACI";
            public static string calismaDurumuKodu = "CALISMADURUMU";
            public static string meslekKodu = "MESLEK";
            public static string isKodu = "IS";
            public static string calismaIstegiKodu = "CALISMAISTEGI";
            public static string sosyalGuvenceKodu = "SOSYALGUVENCE";
            public static string saglikSigortasiKodu = "SAGLIKSIGORTASI";
            public static string saglikDurumuKodu = "SAGLIKDURUMU";
            public static string kronikDurumuKodu = "KRONIKDURUMU";
            public static string duzenliIlacIhtiyaciKodu = "DUZENLIILACIHTIYACI";
            public static string ilacTeminDurumuKodu = "ILACTEMINI";
            public static string psikolojikDestekIhtiyaciKodu = "PSIKOLOJIKDESTEKIHTIYACI";
            public static string psikolojikDestekCozumuKodu = "PSIKOLOJIKDESTEKCOZUMU";
            public static string sagliktaGittigiYerKodu = "SAGLIKTAGITTIGIYER";
            public static string saglikYeriSorunuKodu = "SAGLIKYERISORUNU";
            public static string dogumKontroluIhtiyaciKodu = "DOGUMKONTROLUIHTIYACI";
            public static string dogumKontroluIstegiKodu = "DOGUMKONTROLUISTEGI";
            public static string hamilelikKontrolleriKodu = "HAMILELIKKONTROLU";
            public static string asiDurumuKodu = "ASIDURUMU";

        public static string rolTuruBul(long id, string rolTuru)
        {
            if (id == 0)
            {
                return sayiUret().ToString();
            }

            return rolTuru;
        }

        public static string rolTuruHazirla(long id)
        {
            string idStr = id.ToString();
            string sifirlar = GenelHelper.repeat("0", SabitlerHelper.SozlukKodBoyu - rolTuru.Length - idStr.Length);
            return rolTuru + sifirlar + idStr;
        }

        public static long sayiUret()
        {
            Random rg = new Random();
            long sayi = rg.Next(100000000, 999999999);

            return sayi;

        }

        public static IEnumerable<SelectListItem> sozlukTurleriListesi(bsyContext ctx, string secilen="", bool hepsiEkle = false)
        {
            IEnumerable<SelectListItem> turListesi = (ctx.tblSozlukTurleri.OrderBy(item => item.Tur)
              .Select(s => new SelectListItem
              {
                  Value = s.Tur.ToString(),
                  Text = s.Tur,
                  Selected = s.Tur.Equals(secilen)
              })).ToList();

            if (hepsiEkle)
            {
                turListesi = turListesi.Prepend(new SelectListItem { Value = "Hepsi", Text = "_Bütün Hepsi", Selected = false });
            }

            turListesi.Append(new SelectListItem { Value = "Hepsi", Text = "_Bütün Hepsi", Selected = false });

            return turListesi;
        }

        public static IEnumerable<SelectListItem> anaSozlukKalemleriDD(bsyContext ctx, string turu, long secilen = 0, int bosHepsiHicbiri = 0)
        {
            IEnumerable<SelectListItem> sozlukListesi = (ctx.tblAnaSozluk.Where(item => item.Turu == turu).OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id == secilen
              })).ToList();

            SelectListItem hepsiHicbiri = new SelectListItem { Value = "0", Text = "_Bütün Hepsi", Selected = false };
            if (bosHepsiHicbiri == 2)
            {
                new SelectListItem { Value = "0", Text = "_Hiçbiri", Selected = false };
            }

            if (bosHepsiHicbiri > 0)
            {
                sozlukListesi = sozlukListesi.Prepend(hepsiHicbiri);
                sozlukListesi.Append(hepsiHicbiri);
            }

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> sozlukKalemleriDD(bsyContext ctx, string turu, long secilen = 0, int bosHepsiHicbiri=0 )
        {
            IEnumerable<SelectListItem> sozlukListesi = (ctx.tblSozluk.Where(item => item.Turu == turu).OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              })).ToList();

            SelectListItem hepsiHicbiri = new SelectListItem { Value = "0", Text = "_Bütün Hepsi", Selected = false };
            if (bosHepsiHicbiri == 2)
            {
                new SelectListItem { Value = "0", Text = "_Hiçbiri", Selected = false };
            }

            if (bosHepsiHicbiri > 0)
            {
                sozlukListesi = sozlukListesi.Prepend(hepsiHicbiri);
                sozlukListesi.Append(hepsiHicbiri);
            }

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> sehrinIlceleri(bsyContext ctx, long SehirID, long secilen = 0)
        {
            var ilceler = (from sx in ctx.tblSozluk
                          join icx in ctx.tblIlceler on sx.id equals icx.id
                          where icx.SehirID == SehirID
                          select new { sx.id, sx.Aciklama }).ToList();

            ilceler.Add(new { id = (long)0, Aciklama = "_Bütün İlçeler" });

            IEnumerable<SelectListItem> sozlukListesi = ilceler.OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              }).ToList();

            return sozlukListesi;
        }

        public static IEnumerable<SelectListItem> IlceninMahalleleri(bsyContext ctx, long IlceID, long secilen = 0)
        {
            var mahalleler = (from sx in ctx.tblSozluk
                          join icx in ctx.tblMahalleler on sx.id equals icx.id
                          where icx.IlceID == IlceID
                          select new { sx.id, sx.Aciklama }).ToList();

            mahalleler.Add(new { id = (long)0, Aciklama = "_Bütün Mahalleler" });

            IEnumerable<SelectListItem> sozlukListesi = mahalleler.OrderBy(item => item.Aciklama)
              .Select(s => new SelectListItem
              {
                  Value = s.id.ToString(),
                  Text = s.Aciklama,
                  Selected = s.id.Equals(secilen)
              }).ToList();

            return sozlukListesi;
        }

        public static string sozlukAciklama(bsyContext ctx, long id)
        {

            string aciklama = "";
            SOZLUK soz = ctx.tblSozluk.Find(id);
            if (soz != null)
            {
                aciklama = soz.Aciklama;
            }

            return aciklama;
        }

        /*
        public static long sozlukID(bsyContext ctx, SOZLUK sozluk)
        {
            SOZLUK soz = ctx.tblSozluk.Where(sx => sx.Kodu == sozluk.Kodu && sx.Aciklama == sozluk.Aciklama).FirstOrDefault();
            if (soz == null)
            {
                return 0;
            }

            return soz.id;
        }
        */

        public static string sozlukKalemi(bsyContext ctx, long id)
        {
            if (id == 0)
            {
                return "Bütün Hepsi";
            }

            SOZLUK sozluk = ctx.tblSozluk.Find(id);
            if (sozluk == null)
            {
                return "Tanımlanmamış";
            }

            return sozluk.Aciklama;
        }

        public static Kunye KunyeHazirla(bsyContext ctx, long id)
        {
            Kunye kunye = new Kunye();
            long birimID = id;
            long yeniBirim = 0;

            SOZLUK soz = ctx.tblSozluk.Find(birimID);
            while (soz != null)
            {
                yeniBirim = soz.BabaID;
                switch (soz.Turu.Trim())
                {
                    case "KISI":
                        kunye.kunyeID.KisiID = birimID;
                        kunye.kunyeBilgi.AdSoyad = soz.Aciklama;
                        kunye.kunyeID.TCNo = soz.Parametre;
                        kunye.kunyeID.HaneID = KisiHelper.kisiHanesi(ctx, kunye.kunyeID.KisiID);
                        yeniBirim = kunye.kunyeID.HaneID;
                        break;
                    case "HANE":
                        kunye.kunyeID.HaneID = birimID;
                        kunye.kunyeBilgi.Adres = haneAdresi(ctx, birimID);
                        kunye.kunyeID.HaneKODU = soz.Parametre;
                        kunye.kunyeBilgi.haneBilgileri = soz.Aciklama;
                        break;
                    case "MAHALLE":
                        kunye.kunyeID.MahalleID = birimID;
                        kunye.kunyeBilgi.Mahalle = soz.Aciklama;
                        kunye.kunyeID.MahalleKodu = soz.Parametre;
                        break;
                    case "ILCE":
                        kunye.kunyeID.IlceID = birimID;
                        kunye.kunyeBilgi.Ilce = soz.Aciklama;
                        break;
                    case "SEHIR":
                        kunye.kunyeID.SehirID = birimID;
                        kunye.kunyeBilgi.Sehir = soz.Aciklama;
                        break;
                    default:
                        break;
                }

                birimID = yeniBirim;
                soz = ctx.tblSozluk.Find(birimID);
            }

            kunye.kunyeListe = KunyeListeleri(ctx, kunye.kunyeID, 0);

            return kunye;
        }

        public static KunyeListe KunyeListeleri(bsyContext ctx, KunyeID kunyeID, int bosHepsiHicbiri)
        {
            User user = (User)HttpContext.Current.Session["USER"];

            KunyeListe kunyeListe = new KunyeListe();

            kunyeListe.bolgeler = SozlukHelper.sozlukKalemleriDD(ctx, SozlukHelper.bolgeKodu, kunyeID.BolgeID,  bosHepsiHicbiri);
            kunyeListe.sehirler = KullaniciHelper.kullaniciSehirleriDD(ctx, user, kunyeID.SehirID, bosHepsiHicbiri);
            kunyeListe.ilceler = KullaniciHelper.kullaniciIlceleriDD(ctx, user, kunyeID.IlceID, bosHepsiHicbiri);
            kunyeListe.mahalleler = KullaniciHelper.kullaniciMahalleleriDD(ctx, user, kunyeID.MahalleID, bosHepsiHicbiri);

            //kunyeListe.sehirler = SozlukHelper.sozlukKalemleriDD(ctx, SozlukHelper.sehirKodu, kunyeID.SehirID, bosHepsiHicbiri);
            //kunyeListe.ilceler = SozlukHelper.sozlukKalemleriDD(ctx, SozlukHelper.ilceKodu, kunyeID.IlceID, bosHepsiHicbiri);
            //kunyeListe.mahalleler = SozlukHelper.sozlukKalemleriDD(ctx, SozlukHelper.mahalleKodu, kunyeID.MahalleID, bosHepsiHicbiri);

            return kunyeListe;
        }
        public static string haneAdresi(bsyContext ctx, long id)
        {
            HANE hane = ctx.tblHaneler.Find(id);
            string adres = hane.Cadde + ";" + hane.Sokak + ";" + hane.Apartman + "-" + hane.Daire;
            return adres;
        }
        public static string haneAdresi(HANE hane)
        {
            string adres = hane.Cadde + ";" + hane.Sokak + ";" + hane.Apartman + "-" + hane.Daire;
            return adres;
        }

        public static SOZLUK haneSozlugu(bsyContext ctx, HANE hane)
        {
            SOZLUK sozluk = new SOZLUK();
            if (hane.id != 0)
            {
                sozluk = ctx.tblSozluk.Find(hane.id);
            }

            sozluk.Turu = SozlukHelper.haneKodu;
            sozluk.Aciklama = SozlukHelper.haneAdresi(hane);
            sozluk.Parametre = hane.HaneKodu;
            sozluk.BabaID = hane.MahalleID;

            return sozluk;

        }

        public static SOZLUK kisiSozlugu(bsyContext ctx, KISI kisi)
        {
            SOZLUK sozluk = new SOZLUK();
            if (kisi.id != 0)
            {
                sozluk = ctx.tblSozluk.Find(kisi.id);
            }

            sozluk.Turu = SozlukHelper.kisiKodu;
            sozluk.Aciklama = kisi.Ad + " " + kisi.Soyad;
            sozluk.Parametre = kisi.TCNo;
            sozluk.BabaID = 0;   // KisiHane Kaydı Babası 

            return sozluk;

        }

    }


}