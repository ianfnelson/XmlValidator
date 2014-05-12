using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace XmlValidator.Tests
{
    [TestFixture]
    public class ProgramFixture
    {
        private static readonly string DefaultXsdPath = TestFilesHelper.GetTestFilePath("default.xsd");
        private AppSettingsProvider appSettingsProvider;

        [SetUp]
        public void SetUp()
        {
            appSettingsProvider = MockRepository.GenerateMock<AppSettingsProvider>();
            appSettingsProvider.Expect(x => x.DefaultXsdPath).Return(DefaultXsdPath);

            AppSettingsProvider.Current = appSettingsProvider;
        }

        [TearDown]
        public void TearDown()
        {
            AppSettingsProvider.ResetToDefault();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfInvalidDirectoryName()
        {
            // Arrange
            var args = new[] { "asdf:/%!" };

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfInvalidXsdPathSpecified()
        {
            // Arrange
            var args = new[] { "c:\\", "asdf:/%!" };

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfInvalidXsdPathFromConfig()
        {
            // Arrange
            var args = new[] { "c:\\" };

            appSettingsProvider = MockRepository.GenerateMock<AppSettingsProvider>();
            appSettingsProvider.Expect(x => x.DefaultXsdPath).Return("asdf:/%!");

            AppSettingsProvider.Current = appSettingsProvider;

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfNullXsdPathFromConfig()
        {
            // Arrange
            var args = new[] { "c:\\" };

            appSettingsProvider = MockRepository.GenerateMock<AppSettingsProvider>();
            appSettingsProvider.Expect(x => x.DefaultXsdPath).Return(null);

            AppSettingsProvider.Current = appSettingsProvider;

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfNoneExistentXsdPathFromConfig()
        {
            // Arrange
            var args = new[] { "c:\\" };

            appSettingsProvider = MockRepository.GenerateMock<AppSettingsProvider>();
            appSettingsProvider.Expect(x => x.DefaultXsdPath).Return("c:\\" + Guid.NewGuid() + ".xsd");

            AppSettingsProvider.Current = appSettingsProvider;

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfNoArgs()
        {
            // Arrange

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(new List<string>(), out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfNonExistentDirectoryName()
        {
            // Arrange
            var args = new[] { "c:\\" + Guid.NewGuid() };

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfNonExistentXsdPathSpecified()
        {
            // Arrange
            var args = new[] { "c:\\", "c:\\" + Guid.NewGuid() + ".xsd" };

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsFalseIfNullArgs()
        {
            // Arrange

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(null, out arguments);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void TryParseArgs_ReturnsTrueIfXsdNotSpecifiedButConfigIsValid()
        {
            // Arrange
            var args = new[] { "c:\\" };

            // Act
            XmlValidatorArguments arguments;
            var result = Program.TryParseArgs(args, out arguments);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void TryParseArgs_SpecifiedDirectoryReturnedInParsedArguments()
        {
            // Arrange
            var args = new[] { "c:\\" };

            // Act
            XmlValidatorArguments arguments;
            Program.TryParseArgs(args, out arguments);

            // Assert
            arguments.Folder.FullName.Should().Be("c:\\");
        }

        [Test]
        public void TryParseArgs_UsesDefaultXsdFromConfigIfXsdNotSpecified()
        {
            // Arrange
            var args = new[] { "c:\\" };

            // Act
            XmlValidatorArguments arguments;
            Program.TryParseArgs(args, out arguments);

            // Assert
            arguments.Xsd.FullName.Should().Be(DefaultXsdPath);
        }

        [Test]
        public void TryParseArgs_UsesSpecifiedXsdIfAnySpecified()
        {
            // Arrange
            var args = new[] { "c:\\", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testfiles/test.xsd") };

            // Act
            XmlValidatorArguments arguments;
            Program.TryParseArgs(args, out arguments);

            // Assert
            arguments.Xsd.Name.Should().Be("test.xsd");
        }
    }
}