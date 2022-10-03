using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bsy.Models
{
    public class Kunye
    {
        public Kunye()
        {
            kunyeID = new KunyeID();
            kunyeBilgi = new KunyeBilgi();
            kunyeListe = new KunyeListe();
            listeID = new KunyeID();
        }
        public KunyeID kunyeID { get; set; }
        public KunyeBilgi kunyeBilgi { get; set; }
        public KunyeListe kunyeListe { get; set; }
        public KunyeID listeID { get; set; }
    }
}