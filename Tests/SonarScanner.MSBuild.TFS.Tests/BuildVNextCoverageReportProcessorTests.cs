﻿/*
 * SonarScanner for MSBuild
 * Copyright (C) 2016-2019 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarScanner.MSBuild.Common;
using SonarScanner.MSBuild.TFS.Tests.Infrastructure;
using TestUtilities;

namespace SonarScanner.MSBuild.TFS.Tests
{
    [TestClass]
    public class BuildVNextCoverageReportProcessorTests
    {
        public TestContext TestContext { get; set; }
        private static List<string> filesToDelete = new List<string>();

        private const string TRX_PAYLOAD = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                            <x:TestRun id=""4e4e4073-b17c-4bd0-a8bc-051bbc5a63e4"" name=""John@JOHN-DOE 2019-05-22 14:26:54:768"" runUser=""JOHN-DO\John"" xmlns:x=""http://microsoft.com/schemas/VisualStudio/TeamTest/2010"">
                                              <x:ResultSummary outcome=""Completed"">
                                                <x:CollectorDataEntries>
                                                    <x:Collector uri=""datacollector://microsoft/CodeCoverage/2.0"">
                                                        <x:UriAttachments>
                                                            <x:UriAttachment>
                                                                <x:A href=""dummy.coverage"">dummy.coverage</x:A>
                                                           </x:UriAttachment>
                                                        </x:UriAttachments>
                                                    </x:Collector>
                                                </x:CollectorDataEntries>
                                              </x:ResultSummary>
                                           </x:TestRun>";

        [TestMethod]
        public void SearchFallbackShouldBeCalled_IfNoTrxFilesFound()
        {
            var mockSearchFallback = new MockSearchFallback();
            mockSearchFallback.SetReturnedFiles("file1.txt", "file2.txt");
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testSubject = new BuildVNextCoverageReportProcessor(new MockReportConverter(), new TestLogger(), mockSearchFallback);

            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            IEnumerable<string> trxFilePaths = null;
            bool result = testSubject.TryGetTrxFilesAccessor(new Common.AnalysisConfig(), settings, out trxFilePaths);

            // Assert
            result.Should().BeTrue(); // expecting true i.e. carry on even if nothing found
            testSubject.TrxFilesLocated.Should().BeFalse();

            IEnumerable<string> binaryFilePaths = null;
            result = testSubject.TryGetVsCoverageFilesAccessor(new Common.AnalysisConfig(), settings, out binaryFilePaths);
            result.Should().BeTrue();
            binaryFilePaths.Should().BeEquivalentTo("file1.txt", "file2.txt");

            mockSearchFallback.FallbackCalled.Should().BeTrue();
        }

        [TestMethod]
        public void SearchFallbackNotShouldBeCalled_IfTrxFilesFound()
        {
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testResultsDir = Path.Combine(testDir, "TestResults");
            Directory.CreateDirectory(testResultsDir);
            TestUtils.CreateTextFile(testResultsDir, "dummy.trx", "");

            filesToDelete.Add(Path.Combine(testResultsDir, "dummy.trx"));

            var testSubject = new BuildVNextCoverageReportProcessor(new MockReportConverter(), new TestLogger(), mockSearchFallback);

            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            IEnumerable<string> trxFilePaths = null;
            bool result = testSubject.TryGetTrxFilesAccessor(new Common.AnalysisConfig(), settings, out trxFilePaths);

            // 1) Search for TRX files -> results found
            result.Should().BeTrue();
            testSubject.TrxFilesLocated.Should().BeTrue();

            // 2) Now search for .coverage files
            IEnumerable<string> binaryFilePaths = null;
            result = testSubject.TryGetVsCoverageFilesAccessor(new Common.AnalysisConfig(), settings, out binaryFilePaths);
            result.Should().BeTrue();
            binaryFilePaths.Should().BeEmpty();

            mockSearchFallback.FallbackCalled.Should().BeFalse();
        }

        [TestMethod]
        public void ProcessCoverageReports_VsCoverageXmlPathProvided_NotCoverageXmlFileAlreadyPresent_ShouldTryConverting()
        {
            // Arrange
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testResultsDir = Path.Combine(testDir, "TestResults");
            var analysisConfig = new AnalysisConfig { LocalSettings = new AnalysisProperties() };
            var testLogger = new TestLogger();
            Directory.CreateDirectory(testResultsDir);

            var coverageDir = Path.Combine(testResultsDir, "dummy", "In");
            Directory.CreateDirectory(coverageDir);

            TestUtils.CreateTextFile(testResultsDir, "dummy.trx", TRX_PAYLOAD);
            TestUtils.CreateTextFile(coverageDir, "dummy.coverage", "");

            var converter = new MockReportConverter();
            converter.CanConvert = true;

            var testSubject = new BuildVNextCoverageReportProcessor(converter, testLogger, mockSearchFallback);
            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            var coveragePathValue = "ThisIsADummyPath";

            analysisConfig.LocalSettings.Add(new Property { Id = SonarProperties.VsCoverageXmlReportsPaths, Value = coveragePathValue });

            testSubject.Initialise(analysisConfig, settings);

            // Act
            var result = testSubject.ProcessCoverageReports();

            // Assert
            result.Should().BeTrue();
            converter.AssertExpectedNumberOfConversions(1);

            Assert.AreEqual(analysisConfig.GetSettingOrDefault(SonarProperties.VsCoverageXmlReportsPaths, true, null), coveragePathValue);
        }

        [TestMethod]
        public void ProcessCoverageReports_VsTestReportsPathsProvided_ShouldSkipSearching()
        {
            // Arrange
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var analysisConfig = new AnalysisConfig { LocalSettings = new AnalysisProperties() };
            var testLogger = new TestLogger();

            var converter = new MockReportConverter();
            converter.CanConvert = true;

            var testSubject = new BuildVNextCoverageReportProcessor(converter, testLogger, mockSearchFallback);
            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            analysisConfig.LocalSettings.Add(new Property { Id = SonarProperties.VsTestReportsPaths, Value = String.Empty });

            testSubject.Initialise(analysisConfig, settings);

            // Act
            var result = testSubject.ProcessCoverageReports();

            // Assert
            // 1) Property vstestreportPaths provided, we skip the search for trx files.
            result.Should().BeTrue();
            testSubject.TrxFilesLocated.Should().BeFalse();
        }

        [TestMethod]
        public void ProcessCoverageReports_VsCoverageXmlPathProvided_CoverageXmlFileAlreadyPresent_NotShouldTryConverting()
        {

            // Arrange
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testResultsDir = Path.Combine(testDir, "TestResults");
            var analysisConfig = new AnalysisConfig { LocalSettings = new AnalysisProperties() };
            var testLogger = new TestLogger();
            Directory.CreateDirectory(testResultsDir);

            var coverageDir = Path.Combine(testResultsDir, "dummy", "In");
            Directory.CreateDirectory(coverageDir);

            TestUtils.CreateTextFile(testResultsDir, "dummy.trx", TRX_PAYLOAD);
            filesToDelete.Add(Path.Combine(testResultsDir, "dummy.trx"));

            TestUtils.CreateTextFile(coverageDir, "dummy.coverage", "");
            filesToDelete.Add(Path.Combine(coverageDir, "dummy.coverage"));

            TestUtils.CreateTextFile(coverageDir, "dummy.coveragexml", "");
            filesToDelete.Add(Path.Combine(coverageDir, "dummy.coveragexml"));

            var converter = new MockReportConverter();
            converter.CanConvert = true;

            var testSubject = new BuildVNextCoverageReportProcessor(converter, testLogger, mockSearchFallback);
            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            analysisConfig.LocalSettings.Add(new Property { Id = SonarProperties.VsCoverageXmlReportsPaths, Value = String.Empty });

            testSubject.Initialise(analysisConfig, settings);

            try
            {
                // Act
                var result = testSubject.ProcessCoverageReports();

                // Assert
                result.Should().BeTrue();
                converter.AssertConvertNotCalled();
                testLogger.AssertWarningsLogged(0);
            }
            finally
            {
                TestUtils.DeleteTextFile(coverageDir, "dummy.coveragexml");
            }
        }

        [TestMethod]
        public void ProcessCoverageReports_NotVsCoverageXmlPathProvided_CoverageXmlFileAlreadyPresent_NotShouldTryConverting()
        {

            // Arrange
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testResultsDir = Path.Combine(testDir, "TestResults");
            var analysisConfig = new AnalysisConfig { LocalSettings = new AnalysisProperties() };
            var testLogger = new TestLogger();

            Directory.CreateDirectory(testResultsDir);

            var coverageDir = Path.Combine(testResultsDir, "dummy", "In");
            Directory.CreateDirectory(coverageDir);

            TestUtils.CreateTextFile(testResultsDir, "dummy.trx", TRX_PAYLOAD);

            TestUtils.CreateTextFile(coverageDir, "dummy.coverage", "");
            TestUtils.CreateTextFile(coverageDir, "dummy.coveragexml", "");

            filesToDelete.AddRange(new List<string>()
            {
                Path.Combine(testResultsDir, "dummy.trx"),
                Path.Combine(coverageDir, "dummy.coverage")
            });

            var converter = new MockReportConverter();
            converter.CanConvert = true;

            var testSubject = new BuildVNextCoverageReportProcessor(converter, testLogger, mockSearchFallback);
            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            testSubject.Initialise(analysisConfig, settings);

            // Act
            try
            {

                using (new AssertIgnoreScope())
                {
                    var result = testSubject.ProcessCoverageReports();

                    // Assert
                    result.Should().BeTrue();
                }
                converter.AssertConvertNotCalled();
            }
            finally
            {
                TestUtils.DeleteTextFile(coverageDir, "dummy.coveragexml");
            }
        }

        [TestMethod]
        public void ProcessCoverageReports_NotVsCoverageXmlPathProvided_NotCoverageXmlFileAlreadyPresent_ShouldTryConverting()
        {
            // Arrange
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testResultsDir = Path.Combine(testDir, "TestResults");
            var analysisConfig = new AnalysisConfig { LocalSettings = new AnalysisProperties() };
            var testLogger = new TestLogger();

            Directory.CreateDirectory(testResultsDir);

            var coverageDir = Path.Combine(testResultsDir, "dummy", "In");
            Directory.CreateDirectory(coverageDir);

            TestUtils.CreateTextFile(testResultsDir, "dummy.trx", TRX_PAYLOAD);
            filesToDelete.Add(Path.Combine(testResultsDir, "dummy.trx"));

            TestUtils.CreateTextFile(coverageDir, "dummy.coverage", "");
            filesToDelete.Add(Path.Combine(coverageDir, "dummy.coverage"));

            var converter = new MockReportConverter();
            converter.CanConvert = true;

            var testSubject = new BuildVNextCoverageReportProcessor(converter, testLogger, mockSearchFallback);
            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            testSubject.Initialise(analysisConfig, settings);

            // Act
            var result = testSubject.ProcessCoverageReports();

            // Assert
            result.Should().BeTrue();
            converter.AssertConvertCalledAtLeastOnce();
        }

        [TestMethod]
        public void ProcessCoverageReports_NotVsCoverageXmlPathProvided_NotCoverageXmlFileAlreadyPresent_ShouldTryConverting_ConversionFailed()
        {
            // Arrange
            var mockSearchFallback = new MockSearchFallback();
            var testDir = TestUtils.CreateTestSpecificFolder(this.TestContext);
            var testResultsDir = Path.Combine(testDir, "TestResults");
            var analysisConfig = new AnalysisConfig { LocalSettings = new AnalysisProperties() };
            var testLogger = new TestLogger();

            Directory.CreateDirectory(testResultsDir);

            var coverageDir = Path.Combine(testResultsDir, "dummy", "In");
            Directory.CreateDirectory(coverageDir);

            TestUtils.CreateTextFile(testResultsDir, "dummy.trx", TRX_PAYLOAD);

            TestUtils.CreateTextFile(coverageDir, "dummy.coverage", "");

            filesToDelete.AddRange(new List<string>()
            {
                Path.Combine(testResultsDir, "dummy.trx"),
                Path.Combine(coverageDir, "dummy.coverage")
            });

            var converter = new MockReportConverter();
            converter.CanConvert = true;
            converter.ShouldNotFailConversion = false;

            var testSubject = new BuildVNextCoverageReportProcessor(converter, testLogger, mockSearchFallback);
            var settings = new MockBuildSettings
            {
                BuildDirectory = testDir
            };

            testSubject.Initialise(analysisConfig, settings);

            // Act
            var result = testSubject.ProcessCoverageReports();

            // Assert
            result.Should().BeTrue();
            converter.AssertExpectedNumberOfConversions(1);
        }

        [ClassCleanup]
        public static void AddFilesToDeleteToEnv() {

            filesToDelete = filesToDelete.Distinct().ToList();

            var currentEnvValue = Environment.GetEnvironmentVariable("TEST_FILE_TO_DELETE", EnvironmentVariableTarget.User) ?? String.Empty;
            if (!string.IsNullOrEmpty(currentEnvValue))
            {
                currentEnvValue += ";;";
            }

            Environment.SetEnvironmentVariable("TEST_FILE_TO_DELETE", currentEnvValue + string.Join(";;",
                filesToDelete), EnvironmentVariableTarget.User);
        }
    }
}
