using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using QRCoder;
using System.Text;

namespace RailwaySystem.HelperClasses
{
    public class QRCodeBuilder
    {
        private static string QR_CODE_KEY = "E4942A76D363361FBDFB0C37ECE62CD5CB2708C7";

        public string GenerateCodeContent(string details)
        {
            SHA1 sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(details + "|" + QR_CODE_KEY));

            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        public System.Drawing.Bitmap GenerateQRCode(string codeContent)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(codeContent, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);

            return code.GetGraphic(20);
        }
    }
}