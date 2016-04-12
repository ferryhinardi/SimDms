/*
 * Created by SharpDevelop.
 * User: okusnadi
 * Date: 11/16/2012
 * Time: 9:21 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TracerX
{

    #region MyStaticTemplate
    public class MyLogger
    {
        public static readonly Logger Log = Logger.GetLogger("MyLogger");
        public static bool InitLogging()
        {
            Logger.BinaryFileLogging.Directory = @"%EXEDIR%\Logs";
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 8;
            Logger.BinaryFileLogging.MaxSizeMb = 10;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            // Open the output file.
            return Logger.BinaryFileLogging.Open();
        }

        static List<string> MyString = new List<string>();

        public static void InitString(int Num)
        {
            Log.Info("Init number of string");

            for (int i = 0; i < Num; i++)
            {
                MyString.Add("");
            }

        }

        private static void checkList()
        {
            if (MyString.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    MyString.Add("");
                }
            }
        }

        public static string DataString(int index)
        {
            checkList();
            return MyString[index].ToString();
        }

        public static void SetDataString(int index, string _data)
        {
            checkList();
            MyString[index] = _data;
        }

        public static void Debug(string mystr)
        {
            Log.Debug(mystr);
        }

        public static void Info(string mystr)
        {
            Log.Info(mystr);
        }

        public static void Error(string mystr)
        {
            Log.Error(mystr);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        public static string GetCRC32(string rawdata)
        {

            try
            {
                return new Crc32(rawdata).ToString();
            }
            catch (System.Exception ex)
            {

            }

            return "00000000";
        }

        public static string GetCRC32(byte[] rawdata)
        {
            try
            {
                return new Crc32(Encoding.ASCII.GetString(rawdata)).ToString();
            }
            catch (System.Exception ex)
            {

            }

            return "00000000";
        }

        public static string GetMD5(byte[] rawdata)
        {
            try
            {
                return Hash.GetHash(Encoding.ASCII.GetString(rawdata), Hash.HashType.MD5);
            }
            catch (System.Exception ex)
            {

            }

            return "00000000000000000000000000000000";
        }

        public static string GetMD5(string rawdata)
        {
            try
            {
                return Hash.GetHash(rawdata, Hash.HashType.MD5);
            }
            catch (System.Exception ex)
            {

            }

            return "00000000000000000000000000000000";
        }

        public static string GetSHA1(string rawdata)
        {
            try
            {
                return Hash.GetHash(rawdata, Hash.HashType.SHA1);
            }
            catch (System.Exception ex)
            {
            }
            return string.Empty;
        }

        public static string GetSHA256(string rawdata)
        {
            try
            {
                return Hash.GetHash(rawdata, Hash.HashType.SHA256);
            }
            catch (System.Exception ex)
            {
            }
            return string.Empty;
        }

        public static string GetSHA512(string rawdata)
        {
            try
            {
                return Hash.GetHash(rawdata, Hash.HashType.SHA512);
            }
            catch (System.Exception ex)
            {
            }
            return string.Empty;
        }

        public static string Encrypt(string v, string k1, string k2)
        {
            //SecureString pwd, pwd1, pwd2;
            //pwd = "test";

            return Cryptographer.Encrypt(v, k1, k2, "SHA1", 2, "@FRn#82m$AZ^01x+", 256);
        }

        public static string Decrypt(string v, string k1, string k2)
        {
            try
            {
                return Cryptographer.Decrypt(v, k1, k2, "SHA1", 2, "@FRn#82m$AZ^01x+", 256);
            }
            catch (System.Exception ex)
            {

            }
            return string.Empty;
        }

        public static string Encrypt(string v)
        {
            //SecureString pwd, pwd1, pwd2;
            //pwd = "test";

            return Cryptographer.Encrypt(v, "m45t3r#2012", "M45t3r$USD#2012", "SHA1", 2, "@FRn#82m$AZ^01x+", 256);
        }

        public static string Decrypt(string v)
        {
            try
            {
                return Cryptographer.Decrypt(v, "m45t3r#2012", "M45t3r$USD#2012", "SHA1", 2, "@FRn#82m$AZ^01x+", 256);
            }
            catch (System.Exception ex)
            {

            }
            return string.Empty;
        }
    }

    #endregion

    #region Fungsi Terbilang
    public class MyTerbilang
    {
        public static string Hitung(double x)
        {

            string[] bilangan = { "", "satu ", "dua ", "tiga ", "empat ", "lima ", "enam ", "tujuh ", "delapan ", "sembilan ", "sepuluh ", "sebelas " };

            string temp = "";

            if (x < 12)
            {
                temp = bilangan[(int)x];
            }

            else if (x < 20)
            {
                temp = Hitung(x - 10).ToString() + "belas ";
            }
            else if (x < 100)
            {
                temp = Hitung(x / 10) + "puluh " + Hitung(x % 10);
            }

            else if (x < 200)
            {
                temp = "seratus " + Hitung(x - 100);
            }

            else if (x < 1000)
            {
                temp = Hitung(x / 100) + "ratus " + Hitung(x % 100);
            }
            else if (x < 2000)
            {
                temp = "seribu " + Hitung(x - 1000);
            }
            else if (x < 1000000)
            {
                temp = Hitung(x / 1000) + "ribu " + Hitung(x % 1000);
            }
            else if (x < 1000000000)
            {
                temp = Hitung(x / 1000000) + "juta " + Hitung(x % 1000000);
            }
            else if (x < 1000000000000)
            {
                temp = Hitung(x / 1000000000) + "milyar " + Hitung(x % 1000000000);
            }

            return temp;

        }

        public static string Rupiah(double x)
        {
            return string.Format("{0}RUPIAH", Hitung(x)).ToUpper();
        }        
    }
    #endregion

    #region Cryptographer
    public class Cryptographer
    {
        public const string KeyElementName = "EncryptionKey";
        public const string EncryptedElementName = "Encrypted";

        //The elements that will be encrypted when the contentEncryption is set to "Credentials" or "All".
        public const string CredentialsElementName = "Credentials";
        public const string AllElementName = "s:Envelope";

        public const int AesKeySize = 256; //The minimum size of the key is 128 bits, and the maximum size is 256 bits. [2]
        public const int RsaKeySize = 1024; //The RSACryptoServiceProvider supports key lengths from 384 bits to 16384 bits in increments of 8 bits if you have the Microsoft Enhanced Cryptographic Provider installed. It supports key lengths from 384 bits to 512 bits in increments of 8 bits if you have the Microsoft Base Cryptographic Provider installed.[1]

        protected const bool Content = false;//Encrypt only the content (true) or the node also (false); it seems not to function on true.?!?

        //on server: generates a new public/private key at its instantiation
        //on client: must be initiated with the public key of the server 
        public static RSACryptoServiceProvider RsaServiceProvider { get; private set; }

        //this is necessary on a multithreading environment to pair the request and reply (both on server and client)
        //on client: contains the message id and the key used to encrypt the request message; it will be used to decrypt the reply message.
        //on server: contains the message is and the key extracted from the encrypted message; it will be used to encrypt the reply message.
        protected static ConcurrentDictionary<string, byte[]> AesKeys { get; private set; }

        static Cryptographer()
        {
            RsaServiceProvider = new RSACryptoServiceProvider(RsaKeySize);
            AesKeys = new ConcurrentDictionary<string, byte[]>();
        }

        public static string Encrypt(string plainText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                            keyBytes,
                                                            initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }


        public static string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         decryptor,
                                                         CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();


            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);
            return plainText;
        }

    }

    public class MD5StringEncoder
    {
        public static string CalcHash(string text)
        {
            MD5 md5Hash = MD5.Create();

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
    #endregion

    #region crc32
    public class Crc32 : HashAlgorithm
    {
        public const UInt32 DefaultPolynomial = 0xedb88320;
        public const UInt32 DefaultSeed = 0xffffffff;

        private UInt32 hash;
        private UInt32 seed;
        private UInt32[] table;
        private static UInt32[] defaultTable;
        private string hashResult;

        public Crc32()
        {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
            hashResult = String.Empty;
        }

        public Crc32(string rawdata)
        {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
            hashResult = String.Empty;
            byte[] array = Encoding.ASCII.GetBytes(rawdata);
            byte[] computehash = this.ComputeHash(array);
            foreach (byte b in computehash) hashResult += b.ToString("x2").ToUpper();
        }

        public override string ToString()
        {
            return hashResult;
        }

        public string CalculateString(string rawdata)
        {
            hashResult = String.Empty;
            byte[] array = Encoding.ASCII.GetBytes(rawdata);
            byte[] computehash = this.ComputeHash(array);
            foreach (byte b in computehash) hashResult += b.ToString("x2").ToUpper();
            return hashResult;
        }



        public Crc32(UInt32 polynomial, UInt32 seed)
        {
            table = InitializeTable(polynomial);
            this.seed = seed;
            Initialize();
        }

        public override void Initialize()
        {
            hash = seed;
        }

        protected override void HashCore(byte[] buffer, int start, int length)
        {
            hash = CalculateHash(table, hash, buffer, start, length);
        }

        protected override byte[] HashFinal()
        {
            byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        public override int HashSize
        {
            get { return 32; }
        }

        public static UInt32 Compute(byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        public static UInt32 Compute(UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        private static UInt32[] InitializeTable(UInt32 polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
                return defaultTable;

            UInt32[] createTable = new UInt32[256];
            for (int i = 0; i < 256; i++)
            {
                UInt32 entry = (UInt32)i;
                for (int j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
                defaultTable = createTable;

            return createTable;
        }

        private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
        {
            UInt32 crc = seed;
            for (int i = start; i < size; i++)
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            return crc;
        }

        private byte[] UInt32ToBigEndianBytes(UInt32 x)
        {
            return new byte[] 
            {
			    (byte)((x >> 24) & 0xff),
			    (byte)((x >> 16) & 0xff),
			    (byte)((x >> 8) & 0xff),
			    (byte)(x & 0xff)
		    };
        }
    }
    #endregion

	
    public class Hash
    {
        public Hash() { }

        public enum HashType : int
        {
            MD5,
            SHA1,
            SHA256,
            SHA512
        }

        public static string GetHash(string text, HashType hashType)
        {
            string hashString;
            switch (hashType)
            {
                case HashType.MD5:
                    hashString = GetMD5(text);
                    break;
                case HashType.SHA1:
                    hashString = GetSHA1(text);
                    break;
                case HashType.SHA256:
                    hashString = GetSHA256(text);
                    break;
                case HashType.SHA512:
                    hashString = GetSHA512(text);
                    break;
                default:
                    hashString = "Invalid Hash Type";
                    break;
            }
            return hashString;
        }

        public static bool CheckHash(string original, string hashString, HashType hashType)
        {
            string originalHash = GetHash(original, hashType);
            return (originalHash == hashString);
        }

        private static string GetMD5(string text)
        {
            MD5 hashString = new MD5CryptoServiceProvider();
            string hex = "";
            byte[] hashValue = hashString.ComputeHash(Encoding.ASCII.GetBytes(text));

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        private static string GetSHA1(string text)
        {
            SHA1Managed hashString = new SHA1Managed();
            string hex = "";
            byte[] hashValue = hashString.ComputeHash(Encoding.ASCII.GetBytes(text));

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        private static string GetSHA256(string text)
        {
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";
            byte[] hashValue = hashString.ComputeHash(Encoding.ASCII.GetBytes(text));

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        private static string GetSHA512(string text)
        {
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            byte[] hashValue = hashString.ComputeHash(Encoding.ASCII.GetBytes(text));

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
    }
    
}
