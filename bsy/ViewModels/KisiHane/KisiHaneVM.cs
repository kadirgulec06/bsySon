using bsy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bsy.ViewModels.KisiHane
{
    public class KisiHaneVM
    {
        public long id { get; set; }
        public Kunye kunyeKisi { get; set; }
        public Kunye kunyeHane { get; set; }
        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }
    }
}