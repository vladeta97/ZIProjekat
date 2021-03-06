﻿using System;
using System.Collections.Generic;

namespace CryptoLib
{
    public class MD5
    {

        #region Fields

        // Shift Per-Round Amount Initialization
        private static readonly int[] ShiftArray = {
            7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
            5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
            4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
            6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
        };

        // Precomputed Key Table Initialization
        private static readonly uint[] KeyArray = {
            0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
            0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
            0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
            0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
            0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
            0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
            0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
            0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
            0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
            0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
            0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
            0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
            0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
            0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
            0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
            0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
        };

        // Constant initialization
        private const uint InitA = 0x67452301;
        private const uint InitB = 0xEFCDAB89;
        private const uint InitC = 0x98BADCFE;
        private const uint InitD = 0x10325476;

        #endregion

        #region Methods

        // Rotates x by c bits to the left
        private static uint LeftRotate(uint x, int c)
        {
            return (x << c) | (x >> (32 - c));
        }

        #endregion

       

        public static byte[] Crypt(byte[] input)
        {
            var a0 = InitA;
            var b0 = InitB;
            var c0 = InitC;
            var d0 = InitD;

            var messageLenBytes = input.Length;
            
            var messageLenBits = messageLenBytes << 3;

            var blockNum = ((messageLenBytes + 8) >> 6) + 1;
           
            var actualLen = blockNum << 6;

            var paddingArray = new byte[actualLen - messageLenBytes];
          
            paddingArray[0] = 0x80;
         
            for (var i = 0; i < 8; i++)
            {
                paddingArray[paddingArray.Length - 8 + i] = (byte) messageLenBits;
                messageLenBits >>= 8;
            }
         
            var finalArray = new byte[actualLen];
            input.CopyTo(finalArray, 0);
            paddingArray.CopyTo(finalArray, messageLenBytes);
            
            var buffer = new int[16];

            for (var i = 0; i < blockNum; i++)
            {
                var a = a0;
                var b = b0;
                var c = c0;
                var d = d0;
                uint f, g;                

                for (var j = 0; j < 16; j++)
                    buffer[j] = (int)BitConverter.ToUInt32(finalArray, i * 64 + j * 4);
                
               
                for (uint j = 0; j < 64; j++)
                {
                    if (j <= 15)
                    {
                        f = (b & c) | (~b & d);
                        g = j;
                    }
                    else if (j <= 31)
                    {
                        f = (d & b) | (~d & c);
                        g = (5 * j + 1) % 16;
                    }
                    else if (j <= 47)
                    {
                        f = b ^ c ^ d;
                        g = (3 * j + 5) % 16;
                    }
                    else
                    {
                        f = c ^ (b | ~d);
                        g = (7 * j) % 16;
                    }

                    var temp = d;
                    d = c;
                    c = b;
                    b = b + LeftRotate((a + f + KeyArray[j] + (uint)buffer[g]), ShiftArray[j]);
                    a = temp;
                }

                
                a0 += a;
                b0 += b;
                c0 += c;
                d0 += d;
            }

            
            var md5 = new byte[16];
            var count = 0;

            for (var i = 0; i < 4; i++)
            {
                var n = (i == 0) ? a0 : ((i == 1) ? b0 : ((i == 2) ? c0 : d0));

                for (var j = 0; j < 4; j++)
                {
                    md5[count++] = (byte) n;
                    n >>= 8;
                }
            }

            return md5;
        }

      

      

    }
}
