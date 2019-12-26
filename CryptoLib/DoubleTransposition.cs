using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CryptoLib
{
    public class DoubleTransposition: ICrypto
    {

        #region Fields

        // Key represented by two strings
        private string[] _key = {"Default", "Value"};

        // Initialization vector used for OFB mode
        private byte[] _iv;

        // Temporary matrix to use for transposition
        private byte[][] _matrix;

        // Array containing alphanumeric characters for generating random keys
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Flag determining whether OFB mode is on
        private bool _outputFeedbackMode;

        // Flag determining whether IV is being crypted to avoid infinite recursion
        private bool _cryptingIV;

        #endregion

        #region Properties

        // Private property for current matrix size
        private int MatrixSize => _key[0].Length * _key[1].Length;

        #endregion

        #region Constructors

        public DoubleTransposition()
        {
            SetKey(GenerateRandomKey());
            _iv = GenerateRandomIV();
;        }

        #endregion

        #region Methods

        // Function that swaps columns of a jagged array matrix
        public static void SwapColumns(int col1, int col2, byte[][] matrix)
        {
            foreach (var t in matrix)
            {
                var temp = t[col1];
                t[col1] = t[col2];
                t[col2] = temp;
            }
        }

        private byte[] CryptIV(byte[] iv)
        {

            _cryptingIV = true;

            // Since the IV is the size of the matrix it will work fine
            var result = Crypt(iv);

            _cryptingIV = false;

            return result;
        }

        #endregion

        #region Interface Methods

        // Key is transmited in the following format: ColumnKey,RowKey
        public bool SetKey(byte[] input)
        {
            var temp = Encoding.ASCII.GetString(input);
            var keyArray = temp.Split(',');
            if (keyArray.Length != 2 || keyArray[0].Length < 2 || keyArray[1].Length < 2) throw
                new ArgumentException("Key provided is too short.");

            _key = keyArray;

            // Generate IV depending on keys
            var sb = new StringBuilder();
            for (var i = 0; i < _key[0].Length; i++)
                sb.Append(_key[1]);
                
            SetIV(Encoding.ASCII.GetBytes(sb.ToString()));
            return true;
        }

        public byte[] GenerateRandomKey()
        {
            var rand = new Random(MatrixSize);
            var k1 = new string(Enumerable.Repeat(CHARS, rand.Next(3, 20)).Select(s => s[rand.Next(s.Length)]).ToArray());
            var k2 = new string(Enumerable.Repeat(CHARS, rand.Next(3, 20)).Select(s => s[rand.Next(s.Length)]).ToArray());
            var key = string.Join(",", k1, k2);
            return Encoding.ASCII.GetBytes(key);
        }

        // Sets the initialization vector for OFB mode
        public bool SetIV(byte[] input)
        {
            if (MatrixSize != input.Length)
                return false;
            _iv = input;
            return true;
        }

        // Generates random initialization vector depending on the size of the matrix
        public byte[] GenerateRandomIV()
        {
            var rand = new Random();
            var b = new byte[MatrixSize];
            rand.NextBytes(b);
            return b;
        }

        // Can be used to enable ofb mode
        public bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments)
        {

            if (specArguments.ContainsKey("ofbModeDT"))
                _outputFeedbackMode = BitConverter.ToBoolean(specArguments["ofbModeDT"], 0);

            return true;
        }


        public byte[] Crypt(byte[] input)
        {
            // Initialize rows and columns
            var columns = _key[0].Length;
            var rows = _key[1].Length;
            
            // Determining number of blocks
            var blockNum = (input.Length % MatrixSize == 0) ? input.Length/ MatrixSize : input.Length/MatrixSize + 1;

            // Result byte array
            var result = new byte[MatrixSize * blockNum];
            
            // IV buffer
            byte[] ivBuffer = _iv.Clone() as byte[];

            using (var stream = new MemoryStream(result))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // Main loop
                    for (var i = 0; i < blockNum; i++)
                    {
                        // Encrypt IV at the start of each block
                        if (_outputFeedbackMode && !_cryptingIV)
                            ivBuffer = CryptIV(ivBuffer);

                        var colKey = _key[0].ToCharArray();
                        var rowKey = _key[1].ToCharArray();

                        // Initialize matrix
                        _matrix = new byte[rows][];
                        for (var j = 0; j < rows; j++)
                            _matrix[j] = new byte[columns];

                        // Forming matrix
                        for (var j = 0; j < rows; j++)
                        for (var k = 0; k < columns; k++)
                        {
                            var index = i * MatrixSize + j * columns + k;

                            // If OFB mode is enabled
                            if (_outputFeedbackMode && !_cryptingIV)
                            {
                                if (index > input.Length - 1)
                                // Value to fill everything else
                                    _matrix[j][k] = (byte) (32 ^ ivBuffer[j * columns + k]);
                                else
                                    _matrix[j][k] = (byte) (input[index] ^ ivBuffer[j * columns + k]);
                                
                            }
                            else
                            {
                                if (index > input.Length - 1)
                                    // Value to fill everything else
                                    _matrix[j][k] = 32;
                                else
                                    _matrix[j][k] = input[index];
                            }

                        }

                        // Sorting key and transposing by columns
                        for (var j = 0; j < columns - 1; j++)
                        {
                            var min = j;

                            for (var index = j + 1; index < columns; index++)
                                if (colKey[index] < colKey[min])
                                    min = index;
                            if (min == j) continue;

                            // Swapping key characters
                            var temp = colKey[j];
                            colKey[j] = colKey[min];
                            colKey[min] = temp;

                            // Swaping columns
                            SwapColumns(j, min, _matrix);
                        }

                        // Sorting key and transposing by rows
                        Array.Sort(rowKey, _matrix);

                        // Writing bytes to result
                        for (var j = 0; j < rows; j++)
                        for (var k = 0; k < columns; k++)
                            writer.Write(_matrix[j][k]);
                    }
                }
            }

            return result;
        }

        public byte[] Decrypt(byte[] output)
        {
            
            // Initialize rows and columns
            var columns = _key[0].Length;
            var rows = _key[1].Length;
            
            if(output.Length % MatrixSize != 0) throw 
                new ArgumentException("Encrypted data must be able to fit in the matrix formed by the keys of Double Transposition.");


            // Initialize StringBuilder object to contain output string
            var sb = new StringBuilder();
            
            // Determining number of blocks
            var blockNum = output.Length / MatrixSize;

            // Result byte array
            var result = new byte[blockNum * MatrixSize];

            // IV buffer
            byte[] ivBuffer = _iv.Clone() as byte[]; ;

            // Main loop
            using (var stream = new MemoryStream(result))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    for (var i = 0; i < blockNum; i++)
                    {
                        // Encrypt IV at the start of each block
                        if (_outputFeedbackMode)
                            ivBuffer = CryptIV(ivBuffer);


                        var colKey = _key[0].ToCharArray();
                        var rowKey = _key[1].ToCharArray();

                        // Generating temporary numeric keys
                        var colNumericKey = Enumerable.Range(1, columns).ToArray();
                        var rowNumericKey = Enumerable.Range(1, rows).ToArray();

                        // Initialize matrix
                        _matrix = new byte[rows][];
                        for (var j = 0; j < rows; j++)
                            _matrix[j] = new byte[columns];

                        // Forming matrix
                        for (var j = 0; j < rows; j++)
                        for (var k = 0; k < columns; k++)
                        {
                            var index = i * MatrixSize + j * columns + k;
                            if (index > output.Length - 1) break;
                            _matrix[j][k] = output[index];
                        }

                        // Sorting keys to get equivalent numeric keys
                        for (var j = 0; j < columns - 1; j++)
                        {
                            var min = j;
                            for (var index = j + 1; index < columns; index++)
                                if (colKey[index] < colKey[min])
                                    min = index;
                            if (min == j) continue;

                            var temp1 = colKey[j];
                            colKey[j] = colKey[min];
                            colKey[min] = temp1;

                            var temp2 = colNumericKey[j];
                            colNumericKey[j] = colNumericKey[min];
                            colNumericKey[min] = temp2;
                        }

                        Array.Sort(rowKey, rowNumericKey);

                        // Sorting numeric key and transposing by rows
                        Array.Sort(rowNumericKey, _matrix);

                        // Sorting numeric key and transposing by columns
                        for (var j = 0; j < columns - 1; j++)
                        {
                            var min = j;

                            for (var index = j + 1; index < columns; index++)
                                if (colNumericKey[index] < colNumericKey[min])
                                    min = index;
                            if (min == j) continue;

                            // Swapping key characters
                            var temp = colNumericKey[j];
                            colNumericKey[j] = colNumericKey[min];
                            colNumericKey[min] = temp;

                            // Swaping columns
                            SwapColumns(j, min, _matrix);
                        }

                        // Writing bytes to result
                        for (var j = 0; j < rows; j++)
                        for (var k = 0; k < columns; k++)
                        {
                            // If OFB mode is enabled
                            if (_outputFeedbackMode)
                            {
                                writer.Write((byte)(_matrix[j][k] ^ ivBuffer[j * columns + k]));

                            }
                            else
                                writer.Write(_matrix[j][k]);
                        }
                    }
                }
            }

            // Create result with no extra spaces
            var pos = result.Length - 1;
            while (result[pos] == 32)
                pos--;

            var finalResult = new byte[pos + 1];
            Array.Copy(result, finalResult, pos + 1);

            return finalResult;
        }

        #endregion
    }
}
