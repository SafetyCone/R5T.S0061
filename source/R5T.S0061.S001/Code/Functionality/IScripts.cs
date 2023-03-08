using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.T0132;
using R5T.T0159.Extensions;

using R5T.S0061.T001;


namespace R5T.S0061.S001
{
    [FunctionalityMarker]
    public partial interface IScripts : IFunctionalityMarker
    {
        public void OpenInstanceFiles()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var instanceVarietyNames = Instances.InstancesVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            var instancesFilePaths = instanceVarietyNames
                .Select(instanceVarietyName =>
                {
                    var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                    var outputFilePath = Instances.PathOperator.GetFilePath(
                        datedOutputDirectoryPath,
                        fileName);

                    return outputFilePath;
                })
                .Now();

            Instances.NotepadPlusPlusOperator.Open_WithDelay(
                500,
                instancesFilePaths);
        }

        public void OutputInstancesToFilesAndOpen()
        {
            /// Inputs.
            var instancesJsonFilePath = Instances.FilePaths.InstancesJsonFilePath;


            /// Run.
            var outputFilePaths = Instances.OutputOperations.OutputInstanceSpecificFiles(
                instancesJsonFilePath);

            Instances.NotepadPlusPlusOperator.Open_WithDelay(
                500,
                outputFilePaths);
        }

        public void UpdateInstancesFile()
        {
            /// Inputs.
            var instancesFilePath = Instances.FilePaths.InstancesJsonFilePath;
            var runInstancesFilePath = Instances.FilePaths.Instances_PerRunJsonFilePath;


            /// Run.
            var (newInstances, removedInstances, _) = Instances.InstanceSetComparisonOperator.CompareRunInstances(
                runInstancesFilePath,
                instancesFilePath,
                Instances.FilePaths.BuildProblemProjectsTextFilePath,
                Instances.FilePaths.ProcessingProblemProjectsTextFilePath);

            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(
                instancesFilePath);

            var updatedInstances = instances
                .AppendRange(newInstances)
                .Except(removedInstances, InstanceDescriptorEqualityComparer.Instance)
                .Now();

            Instances.JsonOperator.Serialize_Synchronous(
                instancesFilePath,
                updatedInstances);

            Instances.NotepadPlusPlusOperator.Open(
                instancesFilePath);
        }

        /// <summary>
        /// A frequent problem is that when a project fails to build or to process, all the instances it contains are considered to have been "removed".
        /// This is incorrect.
        /// Instances should only be considered "removed" when the project in which they exist is successfully built and processed, but no longer contains the instance.
        /// </summary>
        public void CompareInstances()
        {
            /// Inputs.
            var today = Instances.DateOperator.GetToday();


            /// Run.
            var priorToTodayInstancesFilePath = Instances.FilePathOperator.Get_PriorToDateFilePath(
                Instances.FilePaths.InstancesJsonFilePath,
                today);

            // If a "prior-to" instances file for today's date does not exist, create it by copying the instances file to the "prior-to" today's date instances file path.
            var priorToTodayInstancesFileExists = Instances.FileSystemOperator.FileExists(priorToTodayInstancesFilePath);
            if(!priorToTodayInstancesFileExists)
            {
                Instances.FileSystemOperator.CopyFile(
                    Instances.FilePaths.InstancesJsonFilePath,
                    priorToTodayInstancesFilePath);
            }

            var (newInstances, removedInstances, _) = Instances.InstanceSetComparisonOperator.CompareRunInstances(
                Instances.FilePaths.Instances_PerRunJsonFilePath,
                priorToTodayInstancesFilePath,
                Instances.FilePaths.BuildProblemProjectsTextFilePath,
                Instances.FilePaths.ProcessingProblemProjectsTextFilePath);

            // Write out new and removed instances to file paths.
            Instances.JsonOperator.Serialize_Synchronous(
                Instances.FilePaths.NewInstancesJsonFilePath,
                newInstances);

            Instances.JsonOperator.Serialize_Synchronous(
                Instances.FilePaths.RemovedInstancesJsonFilePath,
                removedInstances);

            // Open files.
            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.NewInstancesJsonFilePath,
                Instances.FilePaths.RemovedInstancesJsonFilePath);
        }

        public void SummarizeInstances()
        {
            /// Inputs.
            var instancesFilePath = Instances.FilePaths.Instances_PerRunJsonFilePath;


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

            var instancesJsonFilePath = Instances.FilePaths.Instances_PerRunJsonFilePath;


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
            var searchTerm =
                "GetMethodName"
                //ExampleSearchTerms.Instance.Deploy
                ;
            
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

            await Instances.TextOutputOperator.InTextOutputContext(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(ProcessProject),
                Instances.LogFilePathOperator.GetLogFilePath,
                async textOutput =>
                {
                    await Instances.Operations.ProcessBuiltProjects(
                        projectFilesTuples,
                        Instances.FilePaths.ProcessingProblemsTextFilePath,
                        Instances.FilePaths.ProcessingProblemProjectsTextFilePath,
                        Instances.FilePaths.Instances_PerRunJsonFilePath,
                        textOutput);
                });

            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.ProcessingProblemsTextFilePath,
                Instances.FilePaths.ProcessingProblemProjectsTextFilePath,
                Instances.FilePaths.Instances_PerRunJsonFilePath);
        }

        /// <summary>
        /// Given a list of projects, process each corresponding built assembly.
        /// </summary>
        public async Task ProcessProjects()
        {
            /// Inputs.
            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(
                Instances.FilePaths.ProjectsListTextFilePath);


            /// Run.
            var processingProblemsTextFilePath = Instances.FilePaths.ProcessingProblemsTextFilePath;
            var processingProblemProjectsTextFilePath = Instances.FilePaths.ProcessingProblemProjectsTextFilePath;
            var instancesJsonFilePath = Instances.FilePaths.Instances_PerRunJsonFilePath;

            // Filter out any projects with build problems.
            var projectFilePathsToSkip = Instances.FileOperator.ReadAllLines_Synchronous(
               Instances.FilePaths.BuildProblemProjectsTextFilePath);

            projectFilePaths = projectFilePaths
                .Except(projectFilePathsToSkip)
                .Now();

            var projectFilesTuples = Instances.Operations.CreateProjectFilesTuples(projectFilePaths);

            var (humanOutputTextFilePath, _) = await Instances.TextOutputOperator.InTextOutputContext(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(ProcessProjects),
                Instances.LogFilePathOperator.GetLogFilePath,
                async textOutput =>
                {
                    await Instances.Operations.ProcessBuiltProjects(
                       projectFilesTuples,
                       processingProblemsTextFilePath,
                       processingProblemProjectsTextFilePath,
                       instancesJsonFilePath,
                       textOutput);
                });

            Instances.NotepadPlusPlusOperator.Open(
                processingProblemsTextFilePath,
                processingProblemProjectsTextFilePath,
                instancesJsonFilePath,
                humanOutputTextFilePath);
        }

        /// <summary>
        /// Given a list of projects, try to build each project and collect all problems while building.
        /// </summary>
        public async Task BuildProjectFilePaths()
        {
            /// Inputs.
            var rebuildFailedBuildsToCollectErrors = true;
            var projectFilePaths =
            Instances.FileOperator.ReadAllLines_Synchronous(
                Instances.FilePaths.ProjectsListTextFilePath);


            /// Run.
            var (humanOutputTextFilePath, _) = await Instances.TextOutputOperator.InTextOutputContext(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(BuildProjectFilePaths),
                Instances.LogFilePathOperator.GetLogFilePath,
                async textOutput =>
                {
                    await Instances.Operations.BuildProjectFilePaths(
                        rebuildFailedBuildsToCollectErrors,
                        projectFilePaths,
                        Instances.FilePaths.BuildProblemsTextFilePath,
                        Instances.FilePaths.BuildProblemProjectsTextFilePath,
                        textOutput);
                });

            // Now open files.
            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.BuildProblemsTextFilePath,
                Instances.FilePaths.BuildProblemProjectsTextFilePath,
                humanOutputTextFilePath);
        }

        /// <summary>
        /// Given a list of projects, try to build each project and collect all problems while building.
        /// </summary>
        public async Task BuildProjectFilePath()
        {
            /// Inputs.
            var rebuildFailedBuildsToCollectErrors = true;
            var projectFilePaths =
            Instances.ArrayOperator.From(
                @"C:\Code\DEV\Git\GitHub\davidcoats\D8S.W0004.Private\source\D8S.W0004\D8S.W0004.csproj")
            ;


            /// Run.
            var (humanOutputTextFilePath, _) = await Instances.TextOutputOperator.InTextOutputContext(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(BuildProjectFilePaths),
                Instances.LogFilePathOperator.GetLogFilePath,
                async textOutput =>
                {
                    await Instances.Operations.BuildProjectFilePaths(
                        rebuildFailedBuildsToCollectErrors,
                        projectFilePaths,
                        Instances.FilePaths.BuildProblemsTextFilePath,
                        Instances.FilePaths.BuildProblemProjectsTextFilePath,
                        textOutput);
                });

            // Now open files.
            Instances.NotepadPlusPlusOperator.Open(
                Instances.FilePaths.BuildProblemsTextFilePath,
                Instances.FilePaths.BuildProblemProjectsTextFilePath,
                humanOutputTextFilePath);
        }

        /// <summary>
        /// <inheritdoc cref="F001.Documentation.GetAllProjectFilePaths" path="/summary"/>
        /// Output all project file paths to <see cref="IFilePaths.ProjectsListTextFilePath"/>.
        /// </summary>
        public void GetAllProjectFilePaths()
        {
            /// Inputs.
            var projectsList_AllTextFilePath = Instances.FilePaths.ProjectsList_AllTextFilePath;
            var projectsListTextFilePath = Instances.FilePaths.ProjectsListTextFilePath;
            var doNotBuildProjectsListTextFilePath = Instances.FilePaths.DoNotBuildProjectsListTextFilePath;


            /// Run.
            var (humanOutputTextFilePath, _)
            = Instances.TextOutputOperator.InTextOutputContext_Synchronous(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(GetAllProjectFilePaths),
                Instances.LogFilePathOperator.GetLogFilePath,
                textOutput =>
                {
                    Instances.Operations.GetAllProjectFilePaths(
                        projectsList_AllTextFilePath,
                        projectsListTextFilePath,
                        doNotBuildProjectsListTextFilePath,
                        textOutput);
                });

            Instances.NotepadPlusPlusOperator.Open(
                projectsList_AllTextFilePath,
                doNotBuildProjectsListTextFilePath,
                projectsListTextFilePath,
                humanOutputTextFilePath);
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
                            logger.ToTextOutput());

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
        public void CleanDoNotBuildProjectsListTextFile()
        {
            var filePath = Instances.FilePaths.DoNotBuildProjectsListTextFilePath;

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
