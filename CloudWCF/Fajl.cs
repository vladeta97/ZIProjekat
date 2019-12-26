using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudWCF
{
    public class Fajl
    {
        public virtual int IdFajl { get; set; }
        public virtual string Ime { get; set; }
        public virtual string Hash { get; set; }
        public virtual Korisnik vlasnik { get; set; }
    }
}