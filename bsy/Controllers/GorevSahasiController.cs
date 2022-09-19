using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.GorevSahasi;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace bsy.ContgorevSahasis
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class GorevSahasiController : Controller
    {
        bsyContext context = new bsyContext();

        // GET: GorevSahasi
        public ActionResult Index()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult ListeGorevSahasi(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            // filtre parametrelerini hazırla

            string eposta = "";
            string ad = "";
            string soyad = "";
            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["EPOSTA"] != null)
                {
                    eposta = Request.Params["EPOSTA"];
                }

                if (Request.Params["AD"] != null)
                {
                    ad = Request.Params["AD"];
                }

                if (Request.Params["SOYAD"] != null)
                {
                    soyad = Request.Params["SOYAD"];
                }

            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreEPOSTA"] = eposta.ToUpper();
                Session["filtreAD"] = ad.ToUpper();
                Session["filtreSOYAD"] = soyad.ToUpper();
            }
            else if (rapor == 1)
            {
                eposta = (string)Session["filtreEPOSTA"];
                ad = (string)Session["filtreAD"];
                soyad = (string)Session["filtreSOYAD"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            DateTime simdi = DateTime.Now;
            //pageSize = 5;
            var query = (from kx in context.tblKullanicilar
                         join gs in context.tblGorevSahasi on kx.id equals gs.userID into gsj
                         from gse in gsj.DefaultIfEmpty()
                         where
                            (kx.eposta + "").Contains(eposta) ||
                            (kx.Ad + "").Contains(ad) ||
                            (kx.Soyad + "").Contains(soyad)
                         select new
                         {
                             id = gse == null ? 0 : gse.id,
                             userID=kx.id,
                             SehirID = gse == null ? 0 : gse.SehirID,
                             IlceID = gse == null ? 0 : gse.IlceID,
                             MahalleID = gse == null ? 0 : gse.MahalleID,
                             kx.eposta,
                             kx.Ad,
                             kx.Soyad,
                             BasTar = gse == null ? simdi : gse.BasTar,
                             BitTar = gse == null ? simdi : gse.BitTar                             
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("Ad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kr in resultSetAfterOrderandPaging
                             join sx in context.tblSozluk on kr.SehirID equals sx.id into sxj
                             from sxde in sxj.DefaultIfEmpty()
                             join sy in context.tblSozluk on kr.IlceID equals sy.id into syj
                             from syde in syj.DefaultIfEmpty()
                             join sz in context.tblSozluk on kr.MahalleID equals sz.id into szj
                             from szde in szj.DefaultIfEmpty()
                             select new
                             {
                                 kr.id,
                                 kr.userID,
                                 kr.SehirID,
                                 kr.IlceID,
                                 kr.MahalleID,
                                 kr.eposta,
                                 kr.Ad,
                                 kr.Soyad,
                                 Sehir = kr.SehirID == 0 ? "Bütün Şehirler" : sxde == null ? "Şehir Hatalı" : sxde.Aciklama,
                                 Ilce = kr.IlceID == 0 ? "Bütün İlçeler" : syde == null ? "İlçe Hatalı" : syde.Aciklama,
                                 Mahalle = kr.MahalleID == 0 ? "Bütün Mahalleler" : szde == null ? "Mahalle Hatalı" : szde.Aciklama,
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
                  from kr in resultSet
                  select new
                  {
                      cell = new string[]
                      {
                                 kr.id.ToString(),
                                 kr.userID.ToString(),
                                 kr.eposta,
                                 kr.Ad,
                                 kr.Soyad,
                                 kr.Sehir,
                                 kr.Ilce,
                                 kr.Mahalle,
                                 kr.Ekle.ToString(),
                                 kr.Degistir.ToString(),
                                 kr.Sil.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YeniGorevSahasi(long id, long userID)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            GorevSahasiVM gorevSahasiVM = GorevSahasiHazirla(id, userID);

            return View(gorevSahasiVM);
        }

        private GorevSahasiVM GorevSahasiHazirla(long id, long userID)
        {
            GorevSahasiVM gorevSahasiVM = new GorevSahasiVM();

            try
            {
                var user = context.tblKullanicilar.Find(userID);

                gorevSahasiVM.id = id;
                gorevSahasiVM.userID = userID;
                gorevSahasiVM.Ad = user.Ad;
                gorevSahasiVM.eposta = user.eposta;
                gorevSahasiVM.Soyad = user.Soyad;

                long sehirID = 0;
                long ilceID = 0;
                long mahalleID = 0;
                var gorevSahasi = context.tblGorevSahasi.Find(id);
                if (gorevSahasi != null)
                {
                    sehirID = gorevSahasi.SehirID;
                    ilceID = gorevSahasi.IlceID;
                    mahalleID = gorevSahasi.MahalleID;

                    gorevSahasiVM.BasTar = gorevSahasi.BasTar;
                    gorevSahasiVM.BitTar = gorevSahasi.BitTar;
                }

                gorevSahasiVM.SehirID = sehirID;
                gorevSahasiVM.IlceID = ilceID;
                gorevSahasiVM.MahalleID = mahalleID;

                gorevSahasiVM.Sehir = SozlukHelper.sozlukKalemi(context, sehirID);
                gorevSahasiVM.Ilce = SozlukHelper.sozlukKalemi(context, ilceID);
                gorevSahasiVM.Mahalle = SozlukHelper.sozlukKalemi(context, mahalleID);

                gorevSahasiVM.sehirler = SozlukHelper.sozlukKalemleriListesi(context, SozlukHelper.sehirKodu, sehirID, true);
                gorevSahasiVM.ilceler = SozlukHelper.sozlukKalemleriListesi(context, SozlukHelper.ilceKodu, ilceID, true);
                gorevSahasiVM.mahalleler = SozlukHelper.sozlukKalemleriListesi(context, SozlukHelper.mahalleKodu, mahalleID, true);
            }
            catch
            {

            }

            return gorevSahasiVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniGorevSahasi(GorevSahasiVM yeniGorevSahasiVM, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniGorevSahasiVM);
            }

            GOREVSAHASI eskiGorevSahasi = context.tblGorevSahasi.Find(yeniGorevSahasiVM.id);
            if (eskiGorevSahasi == null)
            {
                eskiGorevSahasi = new GOREVSAHASI();
            }

            eskiGorevSahasi = VMtoGorevSahasi(eskiGorevSahasi, yeniGorevSahasiVM);

            if (eskiGorevSahasi.id == 0)
            {
                context.tblGorevSahasi.Add(eskiGorevSahasi);
                m = new Mesaj("tamam", "Görev Sahası Kaydı Eklenmiştir.");
            }
            else
            {
                context.Entry(eskiGorevSahasi).State = EntityState.Modified;
                m = new Mesaj("tamam", "Görev Sahası Kaydı Güncellenmiştir.");
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Görev Sahası kaydı güncelleneMEdi");
            }


            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/GorevSahasi/Index", false);
            return Content("OK");

        }

        private GOREVSAHASI VMtoGorevSahasi(GOREVSAHASI gs, GorevSahasiVM gsVM)
        {
            gs.BasTar = gsVM.BasTar;
            gs.BitTar = gsVM.BitTar;
            gs.id = gsVM.id;
            gs.IlceID = gsVM.IlceID;
            gs.MahalleID = gsVM.MahalleID;
            gs.SehirID = gsVM.SehirID;
            gs.userID = gsVM.userID;

            return gs;
        }
        public ActionResult GorevSahasiSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            GOREVSAHASI gorevSahasi = context.tblGorevSahasi.Find(id);
            context.Entry(gorevSahasi).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Görev Sahası Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Görev Sahası Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/GorevSahasi/Index", false);
            return Content("OK");
        }
    }
}