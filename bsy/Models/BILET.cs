using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class BILET
    {
        public int id { get; set; }
        public int UserID { get; set; }

        [MaxLength(400)]
        public string Bilet { get; set; }
    }
}