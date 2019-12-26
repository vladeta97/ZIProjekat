using System.Collections.Generic;

namespace CryptoLib
{
    public interface ICrypto
    {

        #region Interface Methods

        byte[] Crypt(byte[] input);

        
        byte[] Decrypt(byte[] output);

        #endregion

    }
}
