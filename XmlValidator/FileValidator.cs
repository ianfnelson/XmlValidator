using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace XmlValidator
{
    public interface IFileValidator
    {
        bool Validate(string filePath, string xsdPath, out List<string> messages);
    }

    public class FileValidator : IFileValidator
    {
        public bool Validate(string filePath, string xsdPath, out List<string> messages)
        {
            bool isValid;
            var errors = new List<string>();

            try
            {
                var settings = BuildXmlReaderSettings(xsdPath, errors);

                using (var xmlValidatingReader = XmlReader.Create(filePath, settings))
                {
                    while (xmlValidatingReader.Read())
                    {
                    }
                }

                isValid = !errors.Any();
            }
            catch (Exception error)
            {
                isValid = false;
                errors.Add(string.Format("Unhandled exception during validation - {0}", error.Message));
            }

            messages = errors;
            return isValid;
        }

        private XmlReaderSettings BuildXmlReaderSettings(string xsdPath, ICollection<string> errors)
        {
            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };

            settings.ValidationFlags |=
                XmlSchemaValidationFlags.ProcessSchemaLocation |
                XmlSchemaValidationFlags.ReportValidationWarnings |
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.AllowXmlAttributes;

            settings.ValidationEventHandler += (sender, e) => errors.Add(FormatValidationMessage(e));

            settings.Schemas.Add(null, XmlReader.Create(xsdPath));
            return settings;
        }

        private static string FormatValidationMessage(ValidationEventArgs e)
        {
            return string.Format("{0} @ line {1} position {2}: {3} \r\n",
                (e.Severity == XmlSeverityType.Error ? "ERROR" : "WARNING"),
                e.Exception.LineNumber,
                e.Exception.LinePosition,
                e.Message);
        }
    }
}