using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;

using R5T.S0061.T001;
using System.Threading.Tasks;

namespace R5T.S0061.S001
{
    [FunctionalityMarker]
    public partial interface IScripts : IFunctionalityMarker
    {
        public void SummarizeInstances()
        {
            /// Inputs.
            var instancesFilePath = Instances.FilePaths.InstancesJsonFilePath;


            /// Run.
            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(
                instancesFilePath);

            Console.WriteLine($"{instances.Length}: count");
        }

        public void SearchFunctionality_NameContainsText_Categorize()
        {
            /// Inputs.
            var searchText = ExampleSearchTerms.Instance.Deploy;
            var includeDraft = true;
            var matchCase = false;

            var instancesJsonFilePath = Instances.FilePaths.InstancesJsonFilePath;


            /// Run.
            var instances = Instances.Operations.LoadInstances_Synchronous(instancesJsonFilePath);

            // Get functionalities, and draft functionalities if desired.
            var functionalityInstances = instances
                .Transform(instances =>
                    Instances.InstanceOperations.WhereIsFunctionality(
                        instances, includeDraft))
                .Now();

            // Determine instances containing text.
            var stringComparision = matchCase
                ? StringComparison.InvariantCulture
                : StringComparison.InvariantCultureIgnoreCase
                ;

            var instancesContainingText = functionalityInstances
                .Where(instance =>
                {
                    // Instance name.
                    var instanceNameContainstext = Instances.Operations.InstanceNameContainsText(
                        instance,
                        searchText,
                        stringComparision);

                    // Project name.
                    var projectNameContainsText = instance.ProjectFilePath.Contains(searchText, stringComparision);

                    // Wait on comments, until I can create functionality to convert the HTML into text.

                    var output = false
                        || instanceNameContainstext
                        || projectNameContainsText
                        ;

                    return output;
                })
                .Now();

            // Categorize.
            //var instancesByCategory = instancesContainingText
            //    // SelectMany since an instance might have text in multiple categories.
            //    .SelectMany(instance =>
            //    {

            //    })
            //    ;


            // Output
            Instances.JsonOperator.Serialize_Synchronous(
                Instances.FilePaths.OutputJsonFilePath,
                instancesContainingText);

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.OutputJsonFilePath);
        }

        public void SearchInstances_NameContainsText()
        {
            /// Inputs.
            var searchTerm = ExampleSearchTerms.Instance.Deploy;
            
            var instancesJsonFilePath = Instances.FilePaths.InstancesJsonFilePath;


            /// Run.
            InstanceDescriptor[] instancesContainingSearchTerm = null;

            var duration = Instances.TimingOperator.InTimingContext(() =>
            {
                var instances = Instances.Operations.LoadInstances_Synchronous(instancesJsonFilePath);

                instancesContainingSearchTerm = Instances.Operations.InstanceNameContainsText(
                    instances,
                    searchTerm)
                    .Now();
            },
            duration =>
            {
                Console.WriteLine($"Search took:\n\t{Instances.DurationFormatters.TotalSeconds_WithMilliseconds(duration)} seconds");
            });

            Instances.JsonOperator.Serialize_Synchronous(
                Instances.FilePaths.OutputJsonFilePath,
                instancesContainingSearchTerm);

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.OutputJsonFilePath);
        }

        public void SummarizeProcessingProblems()
        {
            /// Inputs.
            var processingProblemsTextFilePath = Instances.FilePaths.ProcessingProblemsTextFilePath;


            /// Run.
            var lines = Instances.FileOperator.ReadAllLines_Synchronous(processingProblemsTextFilePath);

            static IEnumerable<(string projectFilePath, string processingProblem)> GetProcessingProblemsByFilePath(IList<string> lines)
            {
                var lineCount = lines.Count;

                // Skip the first three lines of the file.
                for (int iLine = 3; iLine < lineCount; iLine++)
                {
                    var projectFilePath = lines[iLine];

                    iLine++;

                    if(iLine == lineCount)
                    {
                        break;
                    }

                    var problem = String.Empty;
                    while (lines[iLine] != "***")
                    {
                        problem += lines[iLine];

                        iLine++;
                    }

                    iLine++;

                    yield return (projectFilePath, problem);
                }
            }

            var processingProblemAndFilePaths = GetProcessingProblemsByFilePath(lines).Now();

            var groupedByProblem = processingProblemAndFilePaths
                .GroupBy(x => x.processingProblem)
                .Now();

            var projectFilePathsByProcessingProblem = GetProcessingProblemsByFilePath(lines)
                .GroupBy(x => x.processingProblem)
                .OrderBy(x => x.Key)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.projectFilePath));

            var outputLines = projectFilePathsByProcessingProblem
                .SelectMany(x => Instances.EnumerableOperator.From($"{x.Key} ({x.Value.Count()})")
                    .AppendRange(x.Value.Select(y => $"\t{y}")))
                .Now();

            Instances.FileOperator.WriteLines_Synchronous(
                Instances.FilePaths.OutputTextFilePath,
                outputLines);

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.OutputTextFilePath);
        }

        public async Task ProcessProject()
        {
            /// Inupts.
            var projectFileTuple =
                //new ProjectFilesTuple
                //// WebAssembly client, <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">.
                //{
                //    ProjectFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.W0004.Private\source\R5T.W0004.Client\R5T.W0004.Client.csproj",
                //    AssemblyFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.W0004.Private\source\R5T.W0004.Client\bin\publish\wwwroot\_framework\R5T.W0004.Client.dll",
                //    DocumentationFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.W0004.Private\source\R5T.W0004.Client\bin\Release\net6.0\R5T.W0004.Client.xml",
                //}
                //// Razor class library, <Project Sdk="Microsoft.NET.Sdk.Razor">.
                //{
                //    ProjectFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0005\source\R5T.R0005\R5T.R0005.csproj",
                //    AssemblyFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0005\source\R5T.R0005\bin\publish\R5T.R0005.dll",
                //    DocumentationFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0005\source\R5T.R0005\bin\publish\R5T.R0005.xml",
                //}
                Instances.Operations.CreateProjectFilesTuple(
                    //// WebAssembly client.
                    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.W0004.Private\source\R5T.W0004.Client\R5T.W0004.Client.csproj");
                    // Microsoft.AspNetCore.App framework reference.
                    @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0065.Private\source\R5T.E0065.Server\R5T.E0065.Server.csproj");
            ;


            /// Run.
            var projectFilesTuples = Instances.EnumerableOperator.From(projectFileTuple)
                .Now();

            await Instances.LoggingOperator.InConsoleLoggerContext(
                nameof(ProcessProjects),
                async logger =>
                {
                    await Instances.Operations.ProcessBuiltProjects(
                        projectFilesTuples,
                        Instances.FilePaths.ProcessingProblemsTextFilePath,
                        Instances.FilePaths.ProcessingProblemProjectsTextFilePath,
                        Instances.FilePaths.InstancesJsonFilePath,
                        logger);
                });

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.ProcessingProblemsTextFilePath,
                Instances.FilePaths.ProcessingProblemProjectsTextFilePath,
                Instances.FilePaths.InstancesJsonFilePath);
        }

        /// <summary>
        /// Given a list of projects, process each corresponding built assembly.
        /// </summary>
        public async Task ProcessProjects()
        {
            /// Inputs.
            var projectFilePaths =
                Instances.FileOperator.ReadAllLines_Synchronous(
                    Instances.FilePaths.ProjectsListTextFilePath)
                //Instances.ArrayOperator.From(
                //    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0064\source\R5T.E0064.W001\R5T.E0064.W001.csproj")
                //    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0003\source\R5T.R0003\R5T.R0003.csproj")
                //    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.F0062\source\R5T.F0062\R5T.F0062.csproj")
                //    // The assembly 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' has already loaded been loaded into this MetadataLoadContext.
                //    @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0063\source\R5T.E0063\R5T.E0063.csproj")
                ;


            /// Run.
            var projectFilePathsToSkip = Instances.FileOperator.ReadAllLines_Synchronous(
               Instances.FilePaths.DoNotBuildProjectsTextFilePath);

            projectFilePaths = projectFilePaths.Except(projectFilePathsToSkip).Now();

            var projectFilesTuples = Instances.Operations.CreateProjectFilesTuples(projectFilePaths);

            await Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                nameof(ProcessProjects),
                async logger =>
                {
                    await Instances.Operations.ProcessBuiltProjects(
                        projectFilesTuples,
                        Instances.FilePaths.ProcessingProblemsTextFilePath,
                        Instances.FilePaths.ProcessingProblemProjectsTextFilePath,
                        Instances.FilePaths.InstancesJsonFilePath,
                        logger);
                });

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.ProcessingProblemsTextFilePath,
                Instances.FilePaths.ProcessingProblemProjectsTextFilePath,
                Instances.FilePaths.InstancesJsonFilePath);
        }

        /// <summary>
        /// Given a list of projects, try to build each project and collect all problems while building.
        /// </summary>
        public async Task BuildAllProjectFilePaths()
        {
            /// Inputs.
            var rebuildFailedBuildsToCollectErrors = true;
            var projectFilePaths =
            Instances.FileOperator.ReadAllLines_Synchronous(
                Instances.FilePaths.ProjectsListTextFilePath)
            //Instances.ArrayOperator.From(
            ////@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0064\source\R5T.E0064.W001\R5T.E0064.W001.csproj")
            ////@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0003\source\R5T.R0003\R5T.R0003.csproj")
            ////@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.Private.Augustodunum\source\R5T.Private.Augustodunum.Lib\R5T.Private.Augustodunum.Lib.csproj")
            ////@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.Private.Augustodunum\source\R5T.Private.Augustodunum.Lib\R5T.Private.Augustodunum.Lib_NuGet.csproj")
            //    @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0065.Private\source\R5T.E0065.R002\R5T.E0065.R002.csproj")
            ;


            /// Run.
            var projectFilePathsToSkip = Instances.FileOperator.ReadAllLines_Synchronous(
               Instances.FilePaths.DoNotBuildProjectsTextFilePath);

            var projectFilePathsToSkipHash = new HashSet<string>(projectFilePathsToSkip);

            await Instances.LoggingOperator.InConsoleLoggerContext(
                nameof(BuildAllProjectFilePaths),
                async logger =>
                {
                    await Instances.Operations.BuildProjectFilePaths(
                        rebuildFailedBuildsToCollectErrors,
                        projectFilePaths,
                        Instances.FilePaths.BuildProblemsTextFilePath,
                        Instances.FilePaths.BuildProblemProjectsTextFilePath,
                        projectFilePathsToSkipHash,
                        logger);
                });

            // Now open files.
            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.BuildProblemsTextFilePath,
                Instances.FilePaths.BuildProblemProjectsTextFilePath);
        }

        /// <summary>
        /// Given the current code base, what project files are found?
        /// </summary>
        public void GetAllProjectFilePaths()
        {
            /// Inputs.
            var outputTextFilePath = Instances.FilePaths.ProjectsListTextFilePath;


            /// Run.
            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                nameof(GetAllProjectFilePaths),
                logger =>
                {
                    Instances.Operations.GetAllProjectFilePaths(
                        outputTextFilePath,
                        logger);
                });

            Instances.NotepadPlusPlusOperator.Open(
                outputTextFilePath);
        }

        /// <summary>
        /// For some projects, there is a _NuGet project in the same directory.
        /// Find all these _NuGet projects so they can be added to the do-not-build list.
        /// </summary>
        public void GetAllNuGetProjectFilePaths()
        {
            /// Inputs.
            var outputTextFilePath = Instances.FilePaths.OutputTextFilePath;


            /// Run.
            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                    nameof(GetAllNuGetProjectFilePaths),
                    logger =>
                    {
                        var projectFilePaths = Instances.Operations.GetAllProjectFilePaths(
                            logger);

                        var nuGetProjectFilePaths = projectFilePaths
                            .Where(projectFilePath => projectFilePath.Contains("_NuGet"))
                            .Now();

                        Instances.FileOperator.WriteLines_Synchronous(
                            outputTextFilePath,
                            nuGetProjectFilePaths);
                    });

            Instances.NotepadPlusPlusOperator.Open(
                outputTextFilePath);
        }

        /// <summary>
        /// Removes duplicates and alphabetizes the list of projects that should not be built.
        /// </summary>
        public void CleanDoNotBuildProjectsFile()
        {
            var filePath = Instances.FilePaths.DoNotBuildProjectsTextFilePath;

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(filePath);

            var cleanedProjectFilePaths = projectFilePaths
                .Distinct()
                .OrderAlphabetically()
                .Now();

            Instances.FileOperator.WriteLines_Synchronous(
                filePath,
                cleanedProjectFilePaths);

            Instances.NotepadPlusPlusOperator.Open(filePath);
        }

        public void DeleteBinAndObjDirectories()
        {
            var projectsListTextFilePath = Instances.FilePaths.OutputTextFilePath;

            var projectFilePaths = Instances.FileOperator.ActuallyReadAllLines_Synchronous(projectsListTextFilePath);

            foreach (var projectFilePath in projectFilePaths)
            {
                var projectDirectoryPath = Instances.PathOperator.GetFileParentDirectoryPath(projectFilePath);
                var binDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                    projectDirectoryPath,
                    "bin");
                var objDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                    projectDirectoryPath,
                    "obj");

                Instances.FileSystemOperator.DeleteDirectory_Idempotent(binDirectoryPath);
                Instances.FileSystemOperator.DeleteDirectory_Idempotent(objDirectoryPath);
            }
        }

        public void ComputeProcessingProblemsThatAreNotBuildProblems()
        {
            var buildProblemsFilePath = Instances.FilePaths.BuildProblemProjectsTextFilePath;
            var processingProblemsFilePath = Instances.FilePaths.ProcessingProblemProjectsTextFilePath;

            var buildProblems = Instances.FileOperator.ReadAllLines_Synchronous(buildProblemsFilePath);
            var processingProblems = Instances.FileOperator.ReadAllLines_Synchronous(processingProblemsFilePath);

            var processingProblemsOnly = processingProblems.Except(buildProblems).Now();

            Instances.FileOperator.WriteAllLines_Synchronous(
                Instances.FilePaths.OutputTextFilePath,
                processingProblemsOnly);

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.OutputTextFilePath);
        }
    }
}
