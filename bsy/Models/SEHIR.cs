using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class SEHIR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        public long bolgeID { get; set; }
        public string Aciklama { get; set; }
    }
}