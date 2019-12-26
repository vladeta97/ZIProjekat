using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using NHibernate;


namespace CloudWCF
{
    public class DataProvider
    {
        public IEnumerable<Korisnik> GetKlijente()
        {
            ISession s = DataLayer.GetSession();
            IEnumerable<Korisnik> klijenti = s.Query<Korisnik>().Select(p => p);
            return klijenti;
        }
    }
}