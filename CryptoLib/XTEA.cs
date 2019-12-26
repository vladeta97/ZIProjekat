using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoLib
{
    public class XTEA: ICrypto
    {

        #region Fields

        // MD5 hash generator used to set the key
       private readonly MD5 _md5 = new MD5();
        //private string textkey="vladetamanic";
        
        // Number of Feistel-cipher rounds
        private uint _rounds;

        // Key used for encrypting/decrypting data
        private byte[] _key;

        // Initialization vector used for OFB mode
        private byte[] _iv;

        // Contains delta value used for encrypting data
        private const uint DELTA = 0x9E377989;

        // Contains recommended rounds value
        private const uint ROUNDS = 32;

        // Flag determining whether OFB mode is on
        private bool _outputFeedbackMode;

        #endregion

        #region Constructors

        public XTEA(bool OFB)
        {
            _rounds = ROUNDS;
            _key = GenerateRandomKey();
            _outputFeedbackMode = setOFB(false);
            _iv = GenerateRandomIV();
        }

        #endregion

        #region Methods

        // Encrypts a block of 64-bit data contained in 2 32-bit values
        private void Crypt(uint[] v, uint[] key)
        {
            uint v0 = v[0], v1 = v[1], sum = 0;
            for (var i = 0; i < _rounds; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                sum += DELTA;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
            }

            v[0] = v0;
            v[1] = v1;
        }

        private void Decrypt(uint[] v, uint[] key)
        {
            uint v0 = v[0], v1 = v[1], sum = DELTA * _rounds;
            for (uint i = 0; i < _rounds; i++)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                sum -= DELTA;
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
            }

            v[0] = v0;
            v[1] = v1;
        }

        #endregion

        #region Interface Methods

        // Key gets converted to MD5 hash
        /*public bool SetKey()
        {
            _key = Encoding.ASCII.GetBytes(textkey);
          
            return true;
        }*/
        
        // Generates random 128-bit key
        public byte[] GenerateRandomKey()
        {
            var rand = new Random();
            var b = new byte[16];
            rand.NextBytes(b);
            return b;
        }

        // Sets 64-bit initialization vector for OFB mode
        public bool SetIV(byte[] input)
        {
            if (input.Length != 8)
                return false;
            _iv = input;
            return true;
        }

        // Generates random 64-bit initialization vector
        public byte[] GenerateRandomIV()
        {
            var rand = new Random();
            var b = new byte[8];
            rand.NextBytes(b);
            return b;
        }

        // Can be used to set round numbers and enable ofb mode
        public bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments)
        {
            if (specArguments.ContainsKey("rounds"))
                _rounds = BitConverter.ToUInt32(specArguments["rounds"], 0);
            if (specArguments.ContainsKey("ofbModeXTEA"))
                _outputFeedbackMode = BitConverter.ToBoolean(specArguments["ofbModeXTEA"], 0);

            return true;
        }
        public bool setOFB(bool set)
        {
            if (set == true)
                _outputFeedbackMode = true;
            else
                _outputFeedbackMode = false;
            return _outputFeedbackMode;
        }

        public byte[] Crypt(byte[] input)
        {

            // Splitting the 128-bit key into four 32-bit values
            var keyBuffer = new[]
            {
                BitConverter.ToUInt32(_key, 0), BitConverter.ToUInt32(_key, 4), BitConverter.ToUInt32(_key, 8),
                BitConverter.ToUInt32(_key, 12)
            };

            // Creating IV uint buffer
            var ivUintBuffer = new uint[2];

            // Converting IV to uint
            Buffer.BlockCopy(_iv, 0, ivUintBuffer, 0, _iv.Length);

            // Array that contains two 32-bit values
            var blockBuffer = new uint[2];

            // Initializing array with padding bytes to make all blocks 64 bits
            var result = new byte[(((input.Length + 4) >> 3) + 1) << 3];
            var lengthBuffer = BitConverter.GetBytes(input.Length);
            Array.Copy(lengthBuffer, result, lengthBuffer.Length);
            Array.Copy(input, 0, result, lengthBuffer.Length, input.Length);

            // Main loop
            using (var stream = new MemoryStream(result))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // i increases by 8 because every block should be 64 bits
                    for (int i = 0; i < result.Length; i += 8)
                    {
                        // First 32 bits of the block into v0, second 32 bits into v1
                        blockBuffer[0] = BitConverter.ToUInt32(result, i);
                        blockBuffer[1] = BitConverter.ToUInt32(result, i + 4);

                        // If OFB mode is enabled
                        if (_outputFeedbackMode)
                        {

                            // Encrypt the IV buffer
                            Crypt(ivUintBuffer, keyBuffer);

                            // Apply XOR to plaintext
                            blockBuffer[0] ^= ivUintBuffer[0];
                            blockBuffer[1] ^= ivUintBuffer[1];
                        }

                        // Encrypt plaintext or encrypted plaintext if OFB mode is on
                        Crypt(blockBuffer, keyBuffer);

                        // Write to result
                        writer.Write(blockBuffer[0]);
                        writer.Write(blockBuffer[1]);
                        
                    }
                }
            }

            return result;
        }

        public byte[] Decrypt(byte[] output)
        {
            // If the encrypted byte array is not divisible by 8 it cannot be decrypted
            if(output.Length % 8 != 0) throw new ArgumentException("Encrypted data length must be multiple of 8 bytes for XTEA.");

            // Splitting the 128-bit key into four 32-bit values
            var keyBuffer = new[]
            {
                BitConverter.ToUInt32(_key, 0), BitConverter.ToUInt32(_key, 4), BitConverter.ToUInt32(_key, 8),
                BitConverter.ToUInt32(_key, 12)
            };

            // Array that contains two 32-bit values
            var blockBuffer = new uint[2];

            // Creating IV uint buffer
            var ivUintBuffer = new uint[2];

            // Converting IV to uint
            Buffer.BlockCopy(_iv, 0, ivUintBuffer, 0, _iv.Length);

            // Placing the byte array into a buffer
            var buffer = new byte[output.Length];
            Array.Copy(output, buffer, output.Length);

            using (var stream = new MemoryStream(buffer))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    for (int i = 0; i < buffer.Length; i += 8)
                    {
                        blockBuffer[0] = BitConverter.ToUInt32(buffer, i);
                        blockBuffer[1] = BitConverter.ToUInt32(buffer, i + 4);
                        
                        // Decrypt to plaintext or to encrypted plaintext if OFB Mode is on
                        Decrypt(blockBuffer, keyBuffer);

                        // If OFB mode is enabled
                        if (_outputFeedbackMode)
                        {

                            // Encrypt the IV buffer
                            Crypt(ivUintBuffer, keyBuffer);

                            // Apply XOR to encrypted text
                            blockBuffer[0] ^= ivUintBuffer[0];
                            blockBuffer[1] ^= ivUintBuffer[1];
                        }

                        writer.Write(blockBuffer[0]);
                        writer.Write(blockBuffer[1]);
                    }
                }
            }

            // Verifying if length is valid
            var length = BitConverter.ToUInt32(buffer, 0);
            if (length > buffer.Length - 4) throw new ArgumentException("Invalid encrypted data");
            var result = new byte[length];
            Array.Copy(buffer, 4, result, 0, length);

            return result;
        }

        #endregion

    }
}
