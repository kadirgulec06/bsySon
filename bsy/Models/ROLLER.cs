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
            userID = 0;
            Tarih = DateTime.Now;
            Rolleri = "";
        }
        public long id { get; set; }
        public long userID { get; set; }
        public DateTime Tarih { get; set; }

        [MaxLength(400)]
        public string Rolleri { get; set; }

    }
}