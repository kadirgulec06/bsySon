using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class MAHALLE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        [MaxLength(50)]
        public string MahalleKodu { get; set; }
        public long IlceID { get; set; }
        public string Aciklama { get; set; }

    }
}