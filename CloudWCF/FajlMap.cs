using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;


namespace CloudWCF
{
    public class FajlMap : ClassMap<Fajl>
    {
        public FajlMap()
        {
            Table("FAJL");

            Schema("S16171");

            Id(x => x.IdFajl, "ID_FAJL").GeneratedBy.Sequence("ID_FAJL_SEQ");

            Map(x => x.Ime, "IME");
            Map(x => x.Hash, "HASH");

            References(x => x.vlasnik).Column("ID_KORISNIK");


        }
    }
}