using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CloudWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string UploadFile(string fileName, byte[] data, int chunkId, long length, bool last, string userName);

        [OperationContract]
        byte[] Download(string fileName, int chunkId, string userName);

        [OperationContract]
        string[] AllFiles(string userName);

        [OperationContract]
        string GetFileHash(string userName, string fileName);

        [OperationContract]
        void SetFileHash(string userName, string fileName, string hash);

        [OperationContract]
        bool Login(string userName, string password);

        [OperationContract]
        bool Register(string userName, string password);

        [OperationContract]
        bool CheckUser(string userName);

        [OperationContract]
        FileInfo FileInfo(string fileName,string userName);

      

    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        [DataMember] public string Name { get; set; }

        [DataMember] public string Extension { get; set; }
    }
}
