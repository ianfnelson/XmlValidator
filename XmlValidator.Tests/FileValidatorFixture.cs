using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace XmlValidator.Tests
{
    [TestFixture]
    public class FileValidatorFixture
    {
        [Test]
        public void FileCannotBeRead_GenericErrorReturned()
        {
            // Arrange
            const string nonExistentXmlFile = "nonexistent.xml";
            var filePath = TestFilesHelper.GetTestFilePath(nonExistentXmlFile);
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            sut.Validate(filePath, xsdPath, out errors);

            // Assert
            errors.Single()
                  .Should()
                  .Contain("Unhandled exception during validation - Could not find file")
                  .And.Contain(nonExistentXmlFile);
        }

        [Test]
        public void FileCannotBeRead_ReturnsFalse()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("nonexistent.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            var result = sut.Validate(filePath, xsdPath, out errors);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void XsdCannotBeRead_GenericErrorReturned()
        {
            // Arrange
            const string nonExistentXsd = "nonexistent.xsd";

            var filePath = TestFilesHelper.GetTestFilePath("test_ok.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath(nonExistentXsd);

            var sut = new FileValidator();

            List<string> errors;

            // Act
            sut.Validate(filePath, xsdPath, out errors);

            // Assert
            errors.Single()
                  .Should()
                  .Contain("Unhandled exception during validation - Could not find file")
                  .And.Contain(nonExistentXsd);
        }

        [Test]
        public void XsdCannotBeRead_ReturnsFalse()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("test_ok.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("nonexistent.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            var result = sut.Validate(filePath, xsdPath, out errors);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void InvalidFile_MultipleErrorsCanBeReturned()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("test_bad2.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            sut.Validate(filePath, xsdPath, out errors);

            // Assert
            errors.ShouldAllBeEquivalentTo(new[]
            {
                "ERROR @ line 2 position 10: The 'Id' element is invalid - The value 'One' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:byte' - The string 'One' is not a valid SByte value. \r\n"
                ,
                "ERROR @ line 4 position 17: The 'Age' element is invalid - The value 'TwentyOne' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:byte' - The string 'TwentyOne' is not a valid SByte value. \r\n"
            });
        }

        [Test]
        public void InvalidFile_ReturnsErrorDetails()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("test_bad1.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            sut.Validate(filePath, xsdPath, out errors);

            // Assert
            errors.ShouldAllBeEquivalentTo(new[]
            {
                "ERROR @ line 2 position 10: The 'Id' element is invalid - The value 'One' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:byte' - The string 'One' is not a valid SByte value. \r\n"
            });
        }

        [Test]
        public void InvalidFile_ReturnsFalse()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("test_bad1.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            var result = sut.Validate(filePath, xsdPath, out errors);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidFile_ReturnsEmptyErrors()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("test_ok.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            sut.Validate(filePath, xsdPath, out errors);

            // Assert
            errors.Should().BeEmpty();
        }

        [Test]
        public void ValidFile_ReturnsTrue()
        {
            // Arrange
            var filePath = TestFilesHelper.GetTestFilePath("test_ok.xml");
            var xsdPath = TestFilesHelper.GetTestFilePath("test.xsd");

            var sut = new FileValidator();

            List<string> errors;

            // Act
            var result = sut.Validate(filePath, xsdPath, out errors);

            // Assert
            result.Should().BeTrue();
        }
    }
}