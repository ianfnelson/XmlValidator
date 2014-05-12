using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;

namespace XmlValidator
{
    public interface IFolderValidator
    {
        void Validate(XmlValidatorArguments arguments);
    }

    public class FolderValidator : IFolderValidator
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Validate(XmlValidatorArguments arguments)
        {
            var inputFiles = arguments.Folder.GetFiles("*.xml", SearchOption.TopDirectoryOnly);

            var successCount = 0;
            var failureCount = 0;

            foreach (var inputFile in inputFiles)
            {
                var validator = new FileValidator();
                List<string> errors;

                if (validator.Validate(inputFile.FullName, arguments.Xsd.FullName, out errors))
                {
                    Log.InfoFormat("File {0} passed validation", inputFile.FullName);
                    successCount++;
                }
                else
                {
                    Log.InfoFormat("File {0} failed validation", inputFile.FullName);
                    foreach (var error in errors)
                    {
                        Log.InfoFormat(error);
                    }
                    failureCount++;
                }
            }

            Log.InfoFormat("Total successes: {0}", successCount);
            Log.InfoFormat("Total failures:  {0}", failureCount);
        }
    }
}