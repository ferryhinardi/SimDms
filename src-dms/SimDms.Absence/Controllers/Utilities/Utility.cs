using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace SimDms.Absence.Controllers.Utilities
{
    public class Utility
    {
        public string GetFileChecksum(string filePath)
        {
            string checksum = "";

            if (string.IsNullOrEmpty(filePath) == false)
            {
                byte[] fileContent = File.ReadAllBytes(filePath);

                using (SHA1CryptoServiceProvider cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    checksum = BitConverter.ToString(cryptoProvider.ComputeHash(fileContent));
                }
            }
            
            return checksum;
        }

        public string GetFileChecksum(byte[] fileContents)
        {
            string checksum = "";

            if (fileContents != null)
            {
                using (SHA1CryptoServiceProvider cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    checksum = BitConverter.ToString(cryptoProvider.ComputeHash(fileContents));
                }
            }

            return checksum;
        }

        public string CalculateSize(int size)
        {
            string[] suffix = {"bytes", "KB", "MB", "GB", "TB" };
            decimal values = size;
            int iterator = 0;
            while ((values / 1024) >= 1)
            {
                values /= 1024;
                iterator++;
            }

            return decimal.Round(values, 2, MidpointRounding.AwayFromZero) + " " + suffix[iterator];
        }

        public ResultModel InitializeResult()
        {
            return new ResultModel()
            {
                status = false,
                message = "",
                details = "",
                data = null
            };
        }
    }
}