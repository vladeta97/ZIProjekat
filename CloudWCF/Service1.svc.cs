using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NHibernate;
using System.Web.SessionState;
using NHibernate.Linq;

namespace CloudWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
       
        //private static byte[] CryptedCloudKey;
        private const string path = "E://Cloude//";
      
        private const int chunkSize = 1048576;
        private static Dictionary<string, byte[]> chunks = new Dictionary<string, byte[]>();

       

        public string[] AllFiles(string userName)
        {
            var path1 = $@"{path}\{userName}";
            Directory.CreateDirectory(path1);
            string[] files = Directory.GetFiles(path1);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            return files;
        }

        public byte[] Download(string fileName, int chunkId,string userName)
        {
            try
            {
                var path1 = $@"{path}\{userName}\{fileName}";
                FileInfo fi = new FileInfo(path1);



                var data = new byte[fi.Length - chunkId * chunkSize < chunkSize ? fi.Length - chunkId * chunkSize : chunkSize];
                //var path2 = $@"{path1}\{fileName}";
                if (chunkId == 0)
                {
                    if (!chunks.ContainsKey(userName))
                        chunks.Add(userName, new byte[fi.Length]);
                    else
                        chunks[userName] = new byte[fi.Length];
                    Buffer.BlockCopy(File.ReadAllBytes(path1), 0, chunks[userName], 0, chunks[userName].Length);
                    Buffer.BlockCopy(chunks[userName], 0, data, 0,
                        data.Length);
                }
                else
                {
                    Buffer.BlockCopy(chunks[userName], chunkId * chunkSize, data, 0,
                        data.Length);
                }
                return data;

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public string UploadFile(string fileName, byte[] data, int chunkId, long length, bool last, string userName)
        {
            try
            {
               var path1= $@"{path}\{userName}";
                if (chunkId == 0)
                {
                    if (!chunks.ContainsKey(userName))
                        chunks.Add(userName, new byte[length]);
                    else
                    {
                        chunks[userName] = new byte[length];
                    }
                    Buffer.BlockCopy(data, 0, chunks[userName], 0,
                        data.Length < chunkSize ? data.Length : chunkSize);
                }
                else
                {
                    Buffer.BlockCopy(data, 0, chunks[userName], chunkId * chunkSize,
                         last ? data.Length : chunkSize);
                }

                if (last)
                {
                    Directory.CreateDirectory(path1);
                    File.WriteAllBytes($@"{path1}\{fileName}", chunks[userName]);
                }
                return "Uploaded";

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public string GetFileHash(string userName, string fileName)
        {
           
            try
            {
                ISession s = DataLayer.GetSession();


                Fajl f = s.Query<Fajl>().Where(e => e.Ime == fileName && e.vlasnik.KorisnickoIme == userName).Select(p => p).FirstOrDefault();

                string hash = f.Hash;

                s.Close();
                return hash;
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec.Message);
                throw;
            }
        }

        public void SetFileHash(string userName, string fileName, string hash)
        {
            ITransaction tx = null;
            try
            {
                ISession s = DataLayer.GetSession();

                Korisnik k=s.Query<Korisnik>().Where(e => e.KorisnickoIme == userName).Select(p => p).FirstOrDefault();

                tx = s.BeginTransaction();

                Fajl d = new Fajl();
                d.Hash = hash;
                d.Ime = fileName;
                d.vlasnik = k;
                s.BeginTransaction();

                s.Save(d);

                tx.Commit();
                // s.Flush();
                // s.Refresh(d);
                s.Close();
                
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec.Message);
                throw;
            }
        }

        public FileInfo FileInfo(string fileName,string userName)
        {
            var path1 = $@"{path}\{userName}\{fileName}";
            return new FileInfo(path1);
        }

        public bool Login(string userName, string password)
        {
            IList<Korisnik> p = new List<Korisnik>();
            try
            {
                ISession s = DataLayer.GetSession();

              
                p = s.CreateQuery("FROM Korisnik").List<Korisnik>();
                foreach (Korisnik d in p)
                {
                    if (d.KorisnickoIme == userName)
                    {
                        if (d.Sifra == password)
                        {
                            return true;
                        }  
                    }
                        
                }
                s.Close();
                return false;
            }
            catch (Exception ec)
            {
                return false;
            }
        }
    

        public bool Register(string userName, string password)
        {
            ITransaction tx = null;
            try
            {
                ISession s = DataLayer.GetSession();
                tx = s.BeginTransaction();
                Korisnik d = new Korisnik();
                d.KorisnickoIme = userName;
                d.Sifra =password;
                s.BeginTransaction();
                s.Save(d);
                tx.Commit();
               // s.Flush();
               // s.Refresh(d);
                s.Close();
                return true;
        }
            catch (Exception ec)
            {
            return false;
            }
        }

        public bool CheckUser(string userName)
        {
            IList<Korisnik> p = new List<Korisnik>();
            try
            {
            
            ISession s = DataLayer.GetSession();
                
                
                p= s.CreateQuery("FROM Korisnik").List<Korisnik>();
                 foreach (Korisnik d in p)
                  {
                    if (d.KorisnickoIme == userName)
                    {
                        //s.Close();
                        return true;
                        
                    }
                  }
                
                s.Close();
                return false;
            }
            catch (Exception ec)
            {
                return false;
            }
        }
    }
}
