using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;

namespace XmlValidator
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            XmlValidatorArguments arguments;
            if (!TryParseArgs(args, out arguments)) return;

            Log.InfoFormat("Folder path: {0}", arguments.Folder);
            Log.InfoFormat("XSD path:    {0}", arguments.Xsd);

            new FolderValidator().Validate(arguments);
        }

        public static bool TryParseArgs(IList<string> args, out XmlValidatorArguments arguments)
        {
            arguments = new XmlValidatorArguments();

            if (args == null || !args.Any())
            {
                Log.Error("Please specify path to folder of files to be validated");
                return false;
            }

            return TryParseFolderPath(args, arguments) && TryParseXsdPath(args, arguments);
        }

        private static bool TryParseXsdPath(IList<string> args, XmlValidatorArguments arguments)
        {
            try
            {
                var xsdPathToParse = args.Count > 1 ? args[1] : AppSettingsProvider.Current.DefaultXsdPath;

                arguments.Xsd = new FileInfo(xsdPathToParse);

                if (!arguments.Xsd.Exists)
                {
                    Log.ErrorFormat("Xsd path {0} does not exist", arguments.Xsd);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error parsing XSD: ", ex);
                return false;
            }
            return true;
        }

        private static bool TryParseFolderPath(IList<string> args, XmlValidatorArguments arguments)
        {
            try
            {
                arguments.Folder = new DirectoryInfo(args[0]);
                if (!arguments.Folder.Exists)
                {
                    Log.ErrorFormat("Folder path {0} does not exist", arguments.Folder);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error parsing folder: ", ex);
                return false;
            }
            return true;
        }
    }

    public class XmlValidatorArguments
    {
        public DirectoryInfo Folder { get; set; }

        public FileInfo Xsd { get; set; }
    }
}