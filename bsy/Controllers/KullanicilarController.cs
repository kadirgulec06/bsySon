using bsy.Helpers;
using bsy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
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

            Mesaj mSifre = null;
            //yeniAO = YeniOzetHesapla(yeniAO, mao);
            eskiUser = UserYeniToEski(eskiUser, yeniUser);           
            if (eskiUser.id == 0)
            {
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
                    string sifre = GenelHelper.sifreUret();
                    mSifre = new Mesaj("bilgi", "Kullanıcı şifresi " + sifre);

                    string sifreliSifre = GenelHelper.CreateSHA512(sifre);
                    eskiUser.Sifre = sifreliSifre;
                    context.tblKullanicilar.Add(eskiUser);
                    m = new Mesaj("tamam", "Kullanıcı Kaydı Eklenmiştir.");

                    bool giriseAcildi = GiriseAcmaYarat(eskiUser);
                    bool sifreDegisme = SifreDegismeYarat(user);

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
                //bool gonderildi = epostaGonder(eskiUser);               
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Kullanıcı kaydı güncelleneMEdi");
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
            GIRISEACMA ga = new GIRISEACMA();

            ga.Aciklama = "İlk kayıt";
            ga.id = 0;
            ga.eposta = user.eposta;
            ga.ip = "";
            ga.Tarih = user.KayitTarihi;

            context.tblGiriseAcma.Add(ga);
            return true;
        }

        private bool SifreDegismeYarat(KULLANICI user)
        {
            SIFREDEGISME sd = new SIFREDEGISME();

            sd.Aciklama = "İlk kayıt";
            sd.id = 0;
            sd.Tarih = user.KayitTarihi;
            sd.userID = user.id;

            context.tblSifreDegisme.Add(sd);
            return true;
        }

        private bool epostaGonder(KULLANICI user)
        {
            string mesaj = "BYS Portalına kaydınız yapılmıştır, şifreniz 12345678";
            string konu = "BYS Portalı Üyeliği";
            bool gonderildi = GenelHelper.sendMail(user.eposta, user.Ad + " " + user.Soyad, konu, mesaj);

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
            eskiUser.Durum = yeniUser.Durum;
            eskiUser.DurumTarihi = yeniUser.DurumTarihi;

            eskiUser.Sifre = GenelHelper.CreateSHA512("123456789");

            return eskiUser;
        }
        public ActionResult KullaniciSil(int idSil)
        {
            int id = idSil;

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
    }
}