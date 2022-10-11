using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class ROLLER
    {
        public ROLLER()
        {
            id = 0;
            UserID = 0;
            Tarih = DateTime.Now;
            Rolleri = "";
        }
        public long id { get; set; }
        public int UserID { get; set; }
        public DateTime Tarih { get; set; }

        [MaxLength(400)]
        public string Rolleri { get; set; }

    }
}