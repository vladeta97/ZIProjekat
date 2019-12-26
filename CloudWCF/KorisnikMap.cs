using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace CloudWCF
{
    public class KorisnikMap : ClassMap<Korisnik>
    {
        public KorisnikMap()
        {
            Table("KORISNIK");

            Schema("S16171");

            Id(x => x.IdKorisnik, "ID_KORISNIK").GeneratedBy.Sequence("ID_KORISNIK_SEQ");

            Map(x => x.KorisnickoIme, "KORISNICKO_IME");
            Map(x => x.Sifra, "SIFRA");

           HasMany(x => x.fajlovi).KeyColumn("ID_FAJL");
        }
    }
}