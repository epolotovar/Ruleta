using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace App.roulette.utility
{
    public sealed class Utilities
    {
        private static Utilities _instance = null;
        private DateTime DateTimeLastValidation { get; set; }

        private bool ValidateDate(DateTime date)
        {
            int hours = (int)(DateTime.Now - (date)).TotalHours;
            return (hours > 0 || hours < 0);
        }

        private Utilities() {}
        public static Utilities Instance
        {
            get
            {
                if (_instance == null || _instance.ValidateDate(_instance.DateTimeLastValidation))
                {
                    _instance = new Utilities();
                }

                return _instance;
            }
        }
        public string CompressGZip(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

            return Convert.ToBase64String(gZipBuffer);
        }
        public string DecompressGZip(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public XDocument ConvertXmlDocumentToXDocument(XmlDocument xml)
        {
            return XDocument.Parse(xml.OuterXml);
        }

        public XmlDocument SerializeToXmlDocument(object _object)
        {
            XmlSerializer _ser = new XmlSerializer(_object.GetType());
            XmlDocument xml = null;
            using (MemoryStream stm = new MemoryStream())
            {
                _ser.Serialize(stm, _object, null);
                stm.Position = 0;
                XmlReaderSettings _settings = new XmlReaderSettings();
                _settings.IgnoreWhitespace = true;
                using (var xtr = XmlReader.Create(stm, _settings))
                {
                    xml = new XmlDocument();
                    xml.Load(xtr);
                }
            }

            return xml;
        }

        public T Deserialize<T>(XmlDocument xml, Type Objeto)
        {
            T result = (T)Deserialize(xml, Objeto);

            return result;
        }

        private static object Deserialize(XmlDocument xml, Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);
            string xmlString = xml.OuterXml.ToString();
            byte[] buffer = ASCIIEncoding.UTF8.GetBytes(xmlString);
            MemoryStream ms = new MemoryStream(buffer);
            XmlReader reader = new XmlTextReader(ms);
            try
            {
                object obj = ser.Deserialize(reader);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
