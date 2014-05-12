using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace XmlValidator
{
    public abstract class AppSettingsProvider
    {
        private static AppSettingsProvider current;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AppSettingsProvider()
        {
            current = new DefaultAppSettingsProvider();
        }

        public static AppSettingsProvider Current
        {
            get { return current; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                current = value;
            }
        }

        public abstract string DefaultXsdPath { get; }

        public static void ResetToDefault()
        {
            current = new DefaultAppSettingsProvider();
        }
    }

    public class DefaultAppSettingsProvider : AppSettingsProvider
    {
        public override string DefaultXsdPath
        {
            get { return ConfigurationManager.AppSettings["DefaultXsdPath"]; }
        }
    }
}