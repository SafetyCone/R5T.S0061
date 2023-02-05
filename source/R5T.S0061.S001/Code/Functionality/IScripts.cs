using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.S001
{
    [FunctionalityMarker]
    public partial interface IScripts : IFunctionalityMarker
    {
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

        /// <summary>
        /// Given a list of projects, process each corresponding built assembly.
        /// </summary>
        public void ProcessProjects()
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
                //    @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0064\source\R5T.E0064.W001\R5T.E0064.W001.csproj")
                ;


            /// Run.
            var projectFilesTuples = Instances.Operations.CreateProjectFilesTuples(projectFilePaths);

            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                nameof(ProcessProjects),
                logger =>
                {
                    Instances.Operations.ProcessBuiltProjects(
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
        public void BuildAllProjectFilePaths()
        {
            /// Inputs.
            var rebuildFailedBuildsToCollectErrors = true;
            var projectFilePaths =
                Instances.FileOperator.ReadAllLines_Synchronous(
                    Instances.FilePaths.ProjectsListTextFilePath)
                //Instances.ArrayOperator.From(
                //    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0064\source\R5T.E0064.W001\R5T.E0064.W001.csproj")
                //    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0003\source\R5T.R0003\R5T.R0003.csproj")
                //    @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.F0062\source\R5T.F0062\R5T.F0062.csproj")
                ;


            /// Run.
            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                nameof(BuildAllProjectFilePaths),
                logger =>
                {
                    Instances.Operations.BuildProjectFilePaths(
                        rebuildFailedBuildsToCollectErrors,
                        projectFilePaths,
                        Instances.FilePaths.BuildProblemsTextFilePath,
                        Instances.FilePaths.BuildProblemProjectsTextFilePath,
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
    }
}
