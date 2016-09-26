using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PCLCrypto;

namespace Privat24Net
{
    internal class RequestBuilder
    {
        public static string GetBalanceRequest(Privat24Credentials credentials, string cardNumber)
        {
            var document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("request",
                    new XAttribute("version", "1.0"),
                    new XElement("merchant",
                        new XElement("id", credentials.MerchantId),
                        new XElement("signature"), ""),
                    new XElement("data",
                        new XElement("oper", "cmt"),
                        new XElement("wait", "0"),
                        new XElement("test", "0"),
                        new XElement("payment",
                            new XAttribute("id", ""),
                            new XElement("prop",
                                new XAttribute("name", "cardnum"),
                                new XAttribute("value", cardNumber)),
                            new XElement("prop",
                                new XAttribute("name", "country"),
                                new XAttribute("value", "UA"))))));

            return SignDocument(document, credentials.Password);
        }

        public static string GetHistoryRequest(Privat24Credentials credentials, string cardNumber, DateTime startDate,
            DateTime endDate)
        {
            var document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("request",
                    new XAttribute("version", "1.0"),
                    new XElement("merchant",
                        new XElement("id", credentials.MerchantId),
                        new XElement("signature"), ""),
                    new XElement("data",
                        new XElement("oper", "cmt"),
                        new XElement("wait", "0"),
                        new XElement("test", "0"),
                        new XElement("payment",
                            new XAttribute("id", ""),
                            new XElement("prop",
                                new XAttribute("name", "sd"),
                                new XAttribute("value", startDate.ToString("dd.MM.yyyy"))),
                            new XElement("prop",
                                new XAttribute("name", "ed"),
                                new XAttribute("value", endDate.ToString("dd.MM.yyyy"))),
                            new XElement("prop",
                                new XAttribute("name", "card"),
                                new XAttribute("value", cardNumber))))));


            return SignDocument(document, credentials.Password);
        }

        private static string SignDocument(XContainer document, string password)
        {
            var dataElements = document?.Element("request")?.Element("data")?.Elements();
            var signatureElement = document?.Element("request")?.Element("merchant")?.Element("signature");
            if (dataElements == null || signatureElement == null)
                return "";

            var dataXml = string.Concat(dataElements.Select(e => e.ToString(SaveOptions.DisableFormatting)));
            signatureElement.Value = ComputeSignature(dataXml, password);
            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static string ComputeSignature(string data, string password)
        {
            var str = data + password;
            var sha1 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1);
            var md5 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5);

            var md5Res = md5.HashData(Encoding.UTF8.GetBytes(str));
            var md5ResString = GetHexadecimalString(md5Res);

            var sha1Res = sha1.HashData(Encoding.UTF8.GetBytes(md5ResString));
            var sha1ResString = GetHexadecimalString(sha1Res);

            return sha1ResString;
        }

        private static string GetHexadecimalString(IEnumerable<byte> buffer)
        {
            return buffer.Select(b => b.ToString("x2")).Aggregate("", (total, cur) => total + cur);
        }
    }
}