using System;

using R5T.T0132;


namespace R5T.S0061.S001
{
    [FunctionalityMarker]
    public partial interface IScripts : IFunctionalityMarker
    {
        /// <summary>
        /// Given a list of projects, process each corresponding built assembly.
        /// </summary>
        public void ProcessProjects()
        {
            /// Inputs.
            var projectFilePaths =
                //Instances.FileOperator.ReadAllLines_Synchronous(
                //    Instances.FilePaths.ProjectsListTextFilePath)
                Instances.ArrayOperator.From(
                    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.E0064\source\R5T.E0064.W001\R5T.E0064.W001.csproj")
                    //@"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.R0003\source\R5T.R0003\R5T.R0003.csproj")
                    @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.F0062\source\R5T.F0062\R5T.F0062.csproj")
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
