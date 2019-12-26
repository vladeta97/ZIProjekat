using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudWCF
{
    public class Korisnik
    {
        public virtual int IdKorisnik { get; set; }
        public virtual string KorisnickoIme { get; set; }
        public virtual string Sifra { get; set; }

        public virtual IList<Fajl> fajlovi { get; set; }

        public Korisnik()
        {
            fajlovi = new List<Fajl>();
        }
    }
}