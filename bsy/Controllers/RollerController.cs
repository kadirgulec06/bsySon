using bsy.Filters;
using bsy.Helpers;
using bsy.Models;
using bsy.ViewModels.Roller;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace bsy.Controllers
{
    [OturumAcikMI]
    [Yetkili(Roles = "YONETICI")]
    public class RollerController : Controller
    {
        bsyContext context = new bsyContext();

        // GET: Roller
        public ActionResult Index()
        {
            ViewBag.IlkGiris = 1;

            return View();
        }

        public ActionResult ListeRolleri(string sidx, string sord, int page, int rows, byte ilkGiris = 0)
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

            //pageSize = 5;
            var query = (from k in context.tblKullanicilar
                         join r in context.tblKullaniciRolleri on k.id equals r.userID into rlj
                         from re in rlj.DefaultIfEmpty()
                         where 
                            (k.eposta + "").Contains(eposta) ||
                            (k.Ad + "").Contains(ad) ||
                            (k.Soyad + "").Contains(soyad)
                         select new
                         {
                             k.id, k.eposta, k.Ad, k.Soyad,
                             rid = re == null ? 0 : re.id,
                             rolleri = re == null ? "" : re.Rolleri
                         });

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var resultSetAfterOrderandPaging = query.OrderBy("Ad").Skip(pageIndex * pageSize).Take(pageSize);

            var resultSet = (from kr in resultSetAfterOrderandPaging
                             select new
                             {
                                 kr.rid,
                                 kr.id,
                                 kr.eposta,
                                 kr.Ad,
                                 kr.Soyad,
                                 kr.rolleri,
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
                                 kr.rid.ToString(),
                                 kr.id.ToString(),
                                 kr.eposta,
                                 kr.Ad,
                                 kr.Soyad,
                                 kr.rolleri,
                                 kr.Degistir.ToString(),
                                 kr.Sil.ToString()
                       }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YeniRolleri(long id, long userID)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            RollerVM rollerVM = RolleriHazirla(id, userID);

            return View(rollerVM);
        }

        private RollerVM RolleriHazirla(long id, long userID)
        {
            RollerVM rolVM = new RollerVM();

            try
            {
                var user = context.tblKullanicilar.Find(userID);

                rolVM.Ad = user.Ad;
                rolVM.eposta = user.eposta;
                rolVM.id = id;
                rolVM.Roller = new List<RolSatiriVM>();
                rolVM.Soyad = user.Soyad;
                rolVM.userID = user.id;

                var roller = (from rs in context.tblSozluk
                              where rs.Turu == SozlukHelper.rolTuru
                              orderby rs.Aciklama
                              select rs.Aciklama).ToList();

                string[] rolleri = KullaniciRolleri(id);
                foreach (string rx in roller)
                {
                    RolSatiriVM rsVM = new RolSatiriVM();
                    rsVM.Rol = rx;
                    rsVM.Secili = 0;
                    if (GenelHelper.dizideVar(rx, rolleri))
                    {
                        rsVM.Secili = 1;
                    }

                    rolVM.Roller.Add(rsVM);
                }
            }
            catch
            {

            }
          
            return rolVM;
        }

        private string[] KullaniciRolleri(long id)
        {
            string birlesikRolleri = "";
            var mevcutRolleri = context.tblKullaniciRolleri.Find(id);
            if (mevcutRolleri != null)
            {
                birlesikRolleri = mevcutRolleri.Rolleri;
            }

            string[] rolleri = birlesikRolleri.Split(',');

            return rolleri;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniRolleri(RollerVM yeniRolleri, string btnSubmit)
        {
            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            if (!ModelState.IsValid)
            {
                return View(yeniRolleri);
            }

            KULLANICIROL eskiRolleri = context.tblKullaniciRolleri.Find(yeniRolleri.id);
            if (eskiRolleri == null)
            {
                eskiRolleri = new KULLANICIROL();
            }

            string birlesikRoller = birlesikRolleri(yeniRolleri.Roller);
            eskiRolleri.Rolleri = birlesikRoller;
            eskiRolleri.Tarih = DateTime.Now;
            eskiRolleri.userID = yeniRolleri.userID;

            if (eskiRolleri.id == 0)
            {
                context.tblKullaniciRolleri.Add(eskiRolleri);
                m = new Mesaj("tamam", "Rol Kaydı Eklenmiştir.");
            }
            else
            {
                context.Entry(eskiRolleri).State = EntityState.Modified;
                m = new Mesaj("tamam", "Rol Kaydı Güncellenmiştir.");
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Rol kaydı güncelleneMEdi");
            }


            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Roller/Index", false);
            return Content("OK");

        }

        private string birlesikRolleri(List<RolSatiriVM> rolleri)
        {
            string birlesikRoller = "";
            foreach(RolSatiriVM rs in rolleri)
            {
                if (rs.Secili == 1)
                {
                    birlesikRoller = birlesikRoller + "," + rs.Rol;
                }
            }
            
            if (birlesikRoller != "")
            {
                birlesikRoller = birlesikRoller.Substring(1);
            }

            return birlesikRoller;
        }

        /*
        private ROLLER RollerYeniToEski(ROLLER eskiRolleri, ROLLER yeniRolleri)
        {

            eskiRolleri.id = yeniRolleri.id;
            eskiRolleri.userID = yeniRolleri.userID;
            eskiRolleri.Rolleri = yeniRolleri.Rolleri;
            eskiRolleri.Tarih = DateTime.Now;

            return eskiRolleri;
        }
        */
        public ActionResult RolleriSil(long idSil)
        {
            long id = idSil;

            List<Mesaj> mesajlar = new List<Mesaj>();
            Mesaj m = null;

            KULLANICIROL roller = context.tblKullaniciRolleri.Find(id);
            context.Entry(roller).State = EntityState.Deleted;

            try
            {
                context.SaveChanges();
                m = new Mesaj("tamam", "Rol Kaydı Silinmiştir.");
            }
            catch (Exception e)
            {
                m = new Mesaj("hata", "Rol Kaydı Silinemedi");
            }

            mesajlar.Add(m);
            Session["MESAJLAR"] = mesajlar;

            Response.Redirect(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/Roller/Index", false);
            return Content("OK");

        }

    }
}