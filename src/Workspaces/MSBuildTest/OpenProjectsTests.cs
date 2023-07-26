// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Roslyn.Test.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.CodeAnalysis.MSBuild.UnitTests
{
    // Generate a list of templates by running `dotnet new list --type project`
    public class OpenProjectsTests : MSBuildWorkspaceTestBase
    {
        // Microsoft.CodeAnalysis.VisualBasic.ERRID
        private const string ERR_UndefinedType1 = "BC30002";

        protected ITestOutputHelper TestOutputHelper { get; set; }

        public OpenProjectsTests(ITestOutputHelper output)
        {
            TestOutputHelper = output;
        }

        [ConditionalTheory(typeof(DotNetSdkMSBuildInstalled), typeof(WindowsOnly))]
        [InlineData("winforms")]
        [InlineData("winformslib")]
        [InlineData("winformscontrollib")]
        [InlineData("wpf")]
        [InlineData("wpflib")]
        [InlineData("wpfcustomcontrollib")]
        [InlineData("wpfusercontrollib")]
        public async Task CSharpTemplateProject_WindowsOnly_LoadWithNoDiagnostics(string templateName)
        {
            var ignoredDiagnostics = templateName switch
            {
                _ => Array.Empty<string>(),
            };

            await AssertTemplateProjectLoadsCleanlyAsync(templateName, LanguageNames.CSharp, ignoredDiagnostics);
        }

        [ConditionalTheory(typeof(DotNetSdkMSBuildInstalled))]
        [InlineData("api")]
        [InlineData("web")]
        [InlineData("grpc")]
        [InlineData("webapi")]
        [InlineData("webapp")]
        [InlineData("mvc")]
        [InlineData("angular")]
        [InlineData("react")]
        [InlineData("blazor")]
        [InlineData("blazorwasm")]
        [InlineData("classlib")]
        [InlineData("console")]
        [InlineData("mstest")]
        [InlineData("nunit")]
        [InlineData("razorclasslib")]
        [InlineData("worker")]
        [InlineData("xunit")]
        public async Task CSharpTemplateProject_LoadWithNoDiagnostics(string templateName)
        {
            var ignoredDiagnostics = templateName switch
            {
                _ => Array.Empty<string>(),
            };

            await AssertTemplateProjectLoadsCleanlyAsync(templateName, LanguageNames.CSharp, ignoredDiagnostics);
        }

        [ConditionalTheory(typeof(DotNetSdkMSBuildInstalled), typeof(WindowsOnly))]
        [InlineData("winforms")]
        [InlineData("winformslib")]
        [InlineData("winformscontrollib")]
        [InlineData("wpf")]
        [InlineData("wpflib")]
        [InlineData("wpfcustomcontrollib")]
        [InlineData("wpfusercontrollib")]
        public async Task VisualBasicTemplateProject_WindowsOnly_LoadWithNoDiagnostics(string templateName)
        {
            var ignoredDiagnostics = templateName switch
            {
                _ => Array.Empty<string>(),
            };

            await AssertTemplateProjectLoadsCleanlyAsync(templateName, LanguageNames.VisualBasic, ignoredDiagnostics);
        }

        [ConditionalTheory(typeof(DotNetSdkMSBuildInstalled))]
        [InlineData("classlib")]
        [InlineData("console")]
        [InlineData("mstest")]
        [InlineData("nunit")]
        [InlineData("xunit")]
        public async Task VisualBasicTemplateProject_LoadWithNoDiagnostics(string templateName)
        {
            var ignoredDiagnostics = (templateName, isWindows: ExecutionConditionUtil.IsWindows) switch
            {
                (_, isWindows: false) => new string[] { ERR_UndefinedType1 },
                _ => Array.Empty<string>(),
            };

            await AssertTemplateProjectLoadsCleanlyAsync(templateName, LanguageNames.VisualBasic, ignoredDiagnostics);
        }

        private async Task AssertTemplateProjectLoadsCleanlyAsync(string templateName, string languageName, string[] ignoredDiagnostics = null)
        {
            try
            {
                if (ignoredDiagnostics is not null)
                {
                    TestOutputHelper.WriteLine($"Ignoring compiler diagnostics: \"{string.Join("\", \"", ignoredDiagnostics)}\"");
                }

                // Clean up previous run
                CleanupProject(templateName, languageName);

                var projectFilePath = GenerateProjectFromTemplate(templateName, languageName, TestOutputHelper);

                await AssertProjectLoadsCleanlyAsync(projectFilePath, ignoredDiagnostics);

                // Clean up successful run
                CleanupProject(templateName, languageName);
            }
            catch
            {
                throw;
            }
        }

        private string GenerateProjectFromTemplate(string templateName, string languageName, ITestOutputHelper outputHelper)
        {
            var projectPath = GetProjectPath(templateName, languageName);
            var projectFilePath = GetProjectFilePath(projectPath, languageName);

            var exitCode = NewProject(templateName, projectPath, languageName, outputHelper);
            Assert.Equal(0, exitCode);

            return projectFilePath;
        }

        private static async Task AssertProjectLoadsCleanlyAsync(string projectFilePath, string[] ignoredDiagnostics)
        {
            using var workspace = CreateMSBuildWorkspace();
            var project = await workspace.OpenProjectAsync(projectFilePath, cancellationToken: CancellationToken.None);

            Assert.Empty(workspace.Diagnostics);

            var compilation = await project.GetCompilationAsync();

            // Unnecessary using directives are reported with a severty of Hidden
            var diagnostics = compilation.GetDiagnostics()
                .Where(diagnostic => diagnostic.Severity > DiagnosticSeverity.Hidden && ignoredDiagnostics?.Contains(diagnostic.Id) != true);

            Assert.Empty(diagnostics);
        }

        private void CleanupProject(string templateName, string languageName)
        {
            var projectPath = GetProjectPath(templateName, languageName);

            if (Directory.Exists(projectPath))
            {
                Directory.Delete(projectPath, true);
            }
        }

        private string GetProjectPath(string templateName, string languageName)
        {
            var languagePrefix = languageName.Replace("#", "Sharp").Replace(' ', '_').ToLower();
            var projectName = $"{languagePrefix}_{templateName}_project";
            return Path.Combine(SolutionDirectory.Path, projectName);
        }

        private static string GetProjectFilePath(string projectPath, string languageName)
        {
            var projectName = Path.GetFileName(projectPath);
            var projectExtension = languageName switch
            {
                LanguageNames.CSharp => "csproj",
                LanguageNames.VisualBasic => "vbproj",
                _ => throw new ArgumentOutOfRangeException(nameof(languageName), actualValue: languageName, message: "Only C# and VB.Net project are supported.")
            };
            return Path.Combine(projectPath, $"{projectName}.{projectExtension}");
        }

        public int NewProject(string templateName, string outputPath, string languageName, ITestOutputHelper output)
        {
            var language = languageName switch
            {
                LanguageNames.CSharp => "C#",
                LanguageNames.VisualBasic => "VB",
                LanguageNames.FSharp => "F#",
                _ => throw new ArgumentOutOfRangeException(nameof(languageName), actualValue: languageName, message: "Only C#, F# and VB.NET project are supported.")
            };

            var restoreResult = RunDotNet($"new \"{templateName}\" -o \"{outputPath}\" --language \"{language}\"");

            output.WriteLine(string.Join(Environment.NewLine, restoreResult.Output));

            return restoreResult.ExitCode;
        }

        private ProcessResult RunDotNet(string arguments)
        {
            Assert.NotNull(DotNetSdkMSBuildInstalled.SdkPath);

            var dotNetExeName = "dotnet" + (Path.DirectorySeparatorChar == '/' ? "" : ".exe");
            var exePath = Path.Combine(DotNetSdkMSBuildInstalled.SdkPath, dotNetExeName);

            return ProcessUtilities.Run(
                exePath, arguments,
                workingDirectory: SolutionDirectory.Path);
        }
    }
}
