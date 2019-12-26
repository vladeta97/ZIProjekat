using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoLib
{
    public class Knapsack: ICrypto
    {

        #region Fields

        // Length of data being crypted in bits
        private const byte DataLength = 8;

        // Keys
        private uint[] _privateKey;
        private uint[] _publicKey;

        // Knapsack variables
        private uint _n;
        private uint _m;
        private uint _mInverse;

        #endregion

        #region Constructors

        public Knapsack()
        {
            // Default values
            _n = 491;
            _m = 41;
            _mInverse = 12;

            _privateKey = new uint[] { 2, 3, 7, 14, 30, 57, 120, 251 };
            _publicKey = new uint[] { 82, 123, 287, 83, 248, 373, 10, 471 };

        }

        #endregion

        #region Methods

        private void ResetPublicKey()
        {
            for (int i = 0; i < DataLength; i++)
                _publicKey[i] = (_privateKey[i] * _m) % _n;
        }

        #endregion

        #region Interface Methods

        public bool SetKey(byte[] input)
        {
            // Convert input into uint array
            uint[] temp = new uint[DataLength];
            Buffer.BlockCopy(input, 0, temp, 0, input.Length);

            // Check if the array is superrising
            float sum = 0;
            for (int i = 0; i < DataLength; i++)
            {
                if (temp[i] <= sum)
                    throw new ArgumentException("Knapsack key needs to be superincreasing");
                sum += temp[i];
            }

            // Set private key
            _privateKey = temp;

            // Set public key
            ResetPublicKey();

            return true;

        }

        public byte[] GenerateRandomKey()
        {
            uint[] temp = new uint[DataLength];
            var sum = _n;
            for (int i = DataLength; i >= 0; i++)
            {
                sum -= 1;
                sum /= 2;
                temp[i] = sum;
            }

            return temp.SelectMany(BitConverter.GetBytes).ToArray();
        }

        public bool SetIV(byte[] input)
        {
            throw new ArgumentException("Knapsack does not use an initialization vector.");
        }

        public byte[] GenerateRandomIV()
        {
            throw new ArgumentException("Knapsack does not use an initialization vector.");
        }

        public bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments)
        {
            if (specArguments.ContainsKey("m"))
                _m = BitConverter.ToUInt32(specArguments["m"], 0);

            if (specArguments.ContainsKey("n"))
                _n = BitConverter.ToUInt32(specArguments["n"], 0);

            if (specArguments.ContainsKey("invm"))
                _mInverse = BitConverter.ToUInt32(specArguments["invm"], 0);

            return true;
        }

        public byte[] Crypt(byte[] input)
        {

            // Getting input length
            var length = input.Length;

            // Initializing result array
            var result = new uint[length];

            // Main loop
            for (var i = 0; i < length; i++)
            {
                // Get value of current byte as byte array
                var inputByte = new [] { input[i] };

                // Get bit array of current byte
                var bits = new BitArray(inputByte);

                // Calculating crypted value for current byte
                for (var j = 0; j < DataLength; j++)
                    if (bits[DataLength- 1 - j])
                        result[i] += _publicKey[j];
            }

            // Converting uint array to string
            var sb = new StringBuilder();
            for (var i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString());
                if (i != result.Length - 1)
                    sb.Append(" ");
            }
            
            // Converting string to byte
            var byteResult = Encoding.ASCII.GetBytes(sb.ToString());

            return byteResult;

        }

        public byte[] Decrypt(byte[] output)
        {
            // Getting input as string array
            var stringOutput = Encoding.ASCII.GetString(output).Split(' ');

            // Getting uint array
            var array = Array.ConvertAll(stringOutput, uint.Parse);

            // Getting input length
            var length = array.Length;

            // Initializing result array
            var result = new byte[length];

            // Main loop
            for (var i = 0; i < length; i++)
            {
                // Calculating transformed crypted data
                var TC = (array[i] * _mInverse) % _n;

                // Temporary bit array
                var bits = new BitArray(DataLength);
                
                // Current value
                var current = TC;

                // Calculating decrypted byte value
                for (var j = DataLength - 1; j >= 0; j--)
                {
                    // Unsigned integers, if private key value is larger there is overflow
                    if (current - _privateKey[j] >= current) continue;
                    bits[DataLength - 1 - j] = true;
                    current -= _privateKey[j];
                }

                // Saving result
                bits.CopyTo(result, i);
            }

            return result;
        }

        #endregion

    }
}
