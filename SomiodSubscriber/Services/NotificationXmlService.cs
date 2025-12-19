using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Security;
using Newtonsoft.Json.Linq;

namespace SomiodSubscriber.Services
{
    public static class NotificationXmlService
    {
        public static string SaveJsonNotificationAsValidatedXml(string topic, string jsonPayload, string appFolder = "AppSubscriber")
        {
            // 1) Parse JSON
            var j = JObject.Parse(jsonPayload);

            string eventType = j["eventType"]?.ToString() ?? "";
            string resourceType = j["resourceType"]?.ToString() ?? "";
            string resourcePath = j["resourcePath"]?.ToString() ?? "";
            string subscription = j["subscription"]?.ToString();
            string timestamp = j["timestamp"]?.ToString() ?? "";

            // 2) Construir XML alinhado com notification.xsd
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            sb.AppendLine("<notification>");
            sb.AppendLine($"  <eventType>{SecurityElement.Escape(eventType)}</eventType>");
            sb.AppendLine($"  <resourceType>{SecurityElement.Escape(resourceType)}</resourceType>");
            sb.AppendLine($"  <resourcePath>{SecurityElement.Escape(resourcePath)}</resourcePath>");
            if (!string.IsNullOrWhiteSpace(subscription))
                sb.AppendLine($"  <subscription>{SecurityElement.Escape(subscription)}</subscription>");
            sb.AppendLine($"  <timestamp>{SecurityElement.Escape(timestamp)}</timestamp>");
            sb.AppendLine("</notification>");

            string xmlString = sb.ToString();

            // 3) Validar XML com XSD
            ValidateXmlString(xmlString);

            // 4) Guardar o XML (só guarda se validar)
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Notifications", appFolder);
            Directory.CreateDirectory(baseDir);

            string fileName = $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss_fff}.xml";
            string fullPath = Path.Combine(baseDir, fileName);

            File.WriteAllText(fullPath, xmlString, Encoding.UTF8);

            return fullPath;
        }
        private static void ValidateXmlString(string xmlString)
        {
            string xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schema", "notification.xsd");

            if (!File.Exists(xsdPath))
                throw new FileNotFoundException("notification.xsd not found. Confirm 'Copy to Output Directory' is set.", xsdPath);

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            settings.Schemas.Add(null, xsdPath);
            settings.ValidationEventHandler += (s, e) =>
            {
                throw new XmlSchemaValidationException(e.Message);
            };

            using (var sr = new StringReader(xmlString))
            using (var reader = XmlReader.Create(sr, settings))
            {
                while (reader.Read()) { }
            }
        }
    }
}
