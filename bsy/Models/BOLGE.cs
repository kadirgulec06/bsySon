using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bsy.Models
{
    public class BOLGE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
        public string Aciklama { get; set; }
    }
}