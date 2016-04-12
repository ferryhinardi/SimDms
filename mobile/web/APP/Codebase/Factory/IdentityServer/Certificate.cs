using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace eXpressAPP
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
            var assembly = typeof(Certificate).Assembly;
            using (var stream = assembly.GetManifestResourceStream("eXISe.idsrv3test.pfx"))
            {
                return new X509Certificate2(ReadStream(stream), "idsrv3test");
            }
        }

        public static X509Certificate2 LoadCertificate(string fileName = "idsrv3test.pfx", string Pwd = "idsrv3test")
        {
            string fileSSL = string.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (Convert.ToBoolean(Settings.FullPath))
            {
                fileSSL = fileName;
            }
            return new X509Certificate2(fileSSL, Pwd);
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}