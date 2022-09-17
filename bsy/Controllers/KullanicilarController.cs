using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
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
    //[OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class KullanicilarController : Controller
    {
        bsyContext context = new bsyContext();

        // GET: Kullanicilar
        public ActionResult ListeKullanicilar(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            //pageSize = 5;
            var query = (from k in context.tblKullanicilar
                         select new
                         {
                             k.Ad,
                             k.Durum,
                             k.DurumTarihi,
                             k.eposta,
                             k.id,
                             k.KayitTarihi,
                             k.KimlikNo,
                             k.Soyad,
                             k.Telefon
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("Ad, Soyad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from k in resultSetAfterOrderandPaging
                             select new
                             {
                                 k.id,
                                 k.eposta,
                                 k.Ad,
                                 k.Soyad,
                                 k.Telefon,
                                 k.KimlikNo,
                                 k.KayitTarihi,
                                 k.Durum,
                                 k.DurumTarihi,
                                 SifreSifirla=0,
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
                                 k.eposta,
                                 k.Ad,
                                 k.Soyad,
                                 k.Telefon,
                                 k.KimlikNo.ToString(),
                                 k.KayitTarihi.ToShortDateString(),
                                 k.Durum,
                                 k.DurumTarihi.ToShortDateString(),
                                 k.SifreSifirla.ToString(),
                                 k.Degistir.ToString(),
                                 k.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult YeniKullanici(long id = 0)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KULLANICI user = null;
            try
            {
                user = context.tblKullanicilar.Find(id);
            }
            catch (Exception ex)
            {

            }

            if (user == null)
            {
                user = new KULLANICI();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniKullanici(KULLANICI yeniUser, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniUser);
            }

            if (!TCKimlikNoValidationAttribute.ValidateKimlikNo(yeniUser.KimlikNo.ToString()))
            {
                m = new Mesaj("hata", "TC Kimlik Numarası Hatalı"); 
                mesajlar.Add(m);
                Session["MESAJLAR"] = mesajlar;
                return View(yeniUser);
            }

            KULLANICI eskiUser = context.tblKullanicilar.Find(yeniUser.id);
            if (eskiUser == null)
            {
                eskiUser = new KULLANICI();
            }

            string sifre = "";
            Mesaj mSifre = null;
            bool yeniKullanici = false;
            bool kaydedildi = true;
            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    eskiUser = UserYeniToEski(eskiUser, yeniUser);
                    if (eskiUser.id == 0)
                    {
                        yeniKullanici = true;
                        KULLANICI user = context.tblKullanicilar.Where(kx => kx.eposta == eskiUser.eposta).FirstOrDefault();
                        if (user != null)
                        {
                            m = new Mesaj("hata", "Bu kullanıcı kaydı daha önce oluşturulmuş, tekrar eklenemez");
                            mesajlar.Add(m);
                            Session["MESAJLAR"] = mesajlar;
                            return View(yeniUser);
                        }
                        else
                        {
                            sifre = SifreHelper.sifreUret();
                            sifre = "abc";
                            mSifre = new Mesaj("bilgi", "Kullanıcı şifresi " + sifre);

                            string sifreliSifre = SifreHelper.CreateSHA512(sifre);
                            eskiUser.Sifre = sifreliSifre;
                            context.tblKullanicilar.Add(eskiUser);
                            m = new Mesaj("tamam", "Kullanıcı Kaydı Eklenmiştir.");
                            context.SaveChanges();

                            bool giriseAcildi = GiriseAcmaYarat(eskiUser);
                            bool sifreSifirlama = SifreHelper.SifreSifirlamaYarat(context, eskiUser.eposta, "İlk Kayıt");

                        }
                    }
                    else
                    {
                        context.Entry(eskiUser).State = EntityState.Modified;
                        m = new Mesaj("tamam", "Kullanıcı Kaydı Güncellenmiştir.");
                    }

                    try
                    {
                        context.SaveChanges();
                        scope.Complete();                        
                        //bool gonderildi = epostaGonder(eskiUser);               
                    }
                    catch (Exception e)
                    {
                        kaydedildi = false;
                        m = new Mesaj("hata", "Kullanıcı kaydı güncelleneMEdi=>" + GenelHelper.exceptionMesaji(e));
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

            if (yeniKullanici && kaydedildi)
            {
                epostaGonder(eskiUser, sifre);
            }

            mesajlar.Add(m);
            if (mSifre != null)
            {
                mesajlar.Add(mSifre);
            }

            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kullanicilar/Index", false);
            return Content("OK");

        }

        private bool GiriseAcmaYarat(KULLANICI user)
        {
            EPOSTAACMA ga = new EPOSTAACMA();

            ga.Aciklama = "İlk kayıt";
            ga.id = 0;
            ga.eposta = user.eposta;
            ga.Tarih = user.KayitTarihi;

            context.tblEPostaAcma.Add(ga);
            return true;
        }

        private bool epostaGonder(KULLANICI user, string sifre)
        {
            bool gonderildi =  SifreHelper.sifrePostasiGonder(context, user.eposta, sifre);

            return gonderildi;
        }
        private KULLANICI UserYeniToEski(KULLANICI eskiUser, KULLANICI yeniUser)
        {
            eskiUser.id = yeniUser.id;
            eskiUser.eposta = yeniUser.eposta;
            eskiUser.Ad = yeniUser.Ad;
            eskiUser.Soyad = yeniUser.Soyad;
            eskiUser.Telefon = yeniUser.Telefon;
            eskiUser.KimlikNo = yeniUser.KimlikNo;
            eskiUser.KayitTarihi = yeniUser.KayitTarihi;
            eskiUser.Durum = "A"; // yeniUser.Durum;
            eskiUser.DurumTarihi = yeniUser.DurumTarihi;

            eskiUser.Sifre = SifreHelper.CreateSHA512("123456789");

            return eskiUser;
        }

        [ValidateAntiForgeryToken]
        public ActionResult KullaniciSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KULLANICI user = context.tblKullanicilar.Find(id);
            context.Entry(user).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Kullanıcı Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Kullanıcı Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kullanicilar/Index", false);
            return Content("OK");
        }
        public ActionResult IPAc()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult ListeKapaliIP(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            // filtre parametrelerini hazırla

            string eposta = "";
            string ip = "";
            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["EPOSTA"] != null)
                {
                    eposta = Request.Params["EPOSTA"];
                }

                if (Request.Params["IP"] != null)
                {
                    ip = Request.Params["IP"];
                }
            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreEPOSTA"] = eposta.ToUpper();
                Session["filtreIP"] = ip.ToUpper();
            }
            else if (rapor == 1)
            {
                eposta = (string)Session["filtreEPOSTA"];
                ip = (string)Session["filtreIP"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            var gdSonHata = from gdx in context.tblGirisDenemeleri
                            where gdx.Durum == false &&
                            ((gdx.eposta + "").Contains(eposta) || (gdx.ip + "").Contains(ip))
                            group gdx by gdx.ip into gdxGRP
                            select gdxGRP.OrderByDescending(gx => gx.Tarih).FirstOrDefault();

            var gdsHataAcmali = (from gdy in gdSonHata
                                 join iax in context.tblIPAcma on gdy.ip equals iax.ip
                                 where iax.Tarih > gdy.Tarih
                                 select gdy.ip).ToList();

            gdSonHata = from gdz in gdSonHata
                        where !gdsHataAcmali.Contains(gdz.ip)
                        select gdz;


            int totalRecords = gdSonHata.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = gdSonHata.OrderBy("Tarih desc, ip").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from k in resultSetAfterOrderandPaging
                             select new
                             {
                                 k.id,
                                 k.eposta,
                                 k.ip,
                                 k.Tarih,
                                 Ac = 0
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
                                 k.ip,
                                 k.Tarih.ToShortDateString(),
                                 k.Ac.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IPHesabiAc(long idAc)
        {
            long id = idAc;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            GIRISDENEME gd = context.tblGirisDenemeleri.Find(id);
            IPACMA ipa = new IPACMA();

            User user = (User)Session["USER"];

            ipa.Aciklama = user.AdSoyad + " tarafından girişlere açıldı";
            ipa.id = 0;
            ipa.ip = gd.ip;
            ipa.Tarih = DateTime.Now;

            context.tblIPAcma.Add(ipa);

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "IP Hesabı girişlere açılmıştır");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "IP Hesabı açılamadı:" + GenelHelper.exceptionMesaji(e));
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kullanicilar/IPAc", false);
            return Content("OK");
        }



        public ActionResult EPostaAc()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult ListeKapaliEPosta(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
        {
            // filtre parametrelerini hazırla

            string eposta = "";
            string ip = "";
            if (Request.Params["_search"] == "true")
            {
                if (Request.Params["EPOSTA"] != null)
                {
                    eposta = Request.Params["EPOSTA"];
                }
            }

            int rapor = 2;
            if (rapor == 0)
            {
                Session["filtreEPOSTA"] = eposta.ToUpper();
            }
            else if (rapor == 1)
            {
                eposta = (string)Session["filtreEPOSTA"];
            }

            int pageIndex = Convert.ToInt32(page) - 1;

            int pageSize = rows;

            var gdSonHata = from gdx in context.tblGirisDenemeleri
                            join kx in context.tblKullanicilar on gdx.eposta equals kx.eposta
                            where gdx.Durum == false &&
                            (gdx.eposta + "").Contains(eposta)
                            group gdx by gdx.eposta into gdxGRP
                            select gdxGRP.OrderByDescending(gx => gx.Tarih).FirstOrDefault();

            var gdsHataAcmali = (from gdy in gdSonHata
                                 join iax in context.tblEPostaAcma on gdy.eposta equals iax.eposta
                                 where iax.Tarih > gdy.Tarih
                                 select gdy.eposta).ToList();

            gdSonHata = from gdz in gdSonHata
                        where !gdsHataAcmali.Contains(gdz.eposta)
                        select gdz;


            int totalRecords = gdSonHata.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = gdSonHata.OrderBy("Tarih desc, Eposta").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from k in resultSetAfterOrderandPaging
                             select new
                             {
                                 k.id,
                                 k.eposta,
                                 k.Tarih,
                                 Ac = 0
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
                                 k.eposta,
                                 k.Tarih.ToShortDateString(),
                                 k.Ac.ToString()
                       }
                  }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EPostaHesabiAc(long idAc)
        {
            long id = idAc;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            GIRISDENEME gd = context.tblGirisDenemeleri.Find(id);
            EPOSTAACMA ipa = new EPOSTAACMA();

            User user = (User)Session["USER"];

            ipa.Aciklama = user.AdSoyad + " tarafından girişlere açıldı";
            ipa.id = 0;
            ipa.eposta = gd.eposta;
            ipa.Tarih = DateTime.Now;

            context.tblEPostaAcma.Add(ipa);

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "EPosta Hesabı girişlere açılmıştır");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "EPosta Hesabı açılamadı:" + GenelHelper.exceptionMesaji(e));
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Kullanicilar/EPostaAc", false);
            return Content("OK");
        }

    }
}