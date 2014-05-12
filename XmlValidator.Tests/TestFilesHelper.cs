using System;
using System.IO;

namespace XmlValidator.Tests
{
    public static class TestFilesHelper
    {
        public static string GetTestFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testfiles\\", fileName);
        }
    }
}