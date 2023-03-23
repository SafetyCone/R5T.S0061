using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using R5T.F0000;
using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker,
        F001.IOperations
    {
        public async Task Run()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            // True, if you want to spend the time to rebuild failed builds in order to collect build errors during this run.
            var rebuildFailedBuildsToCollectErrors = true;


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            await Instances.LoggingOperator.InConsoleLoggerContext(
                Instances.Values.ApplicationName,
                async logger =>
                {
                    var projectsListTextFilePath = this.GetAllProjectFilePaths(
                        datedOutputDirectoryPath,
                        logger);

                    var (buildProblemsFilePath, buildProblemProjectsFilePath) = await this.BuildProjectFilePaths(
                        datedOutputDirectoryPath,
                        logger,
                        rebuildFailedBuildsToCollectErrors,
                        projectsListTextFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var projectFileTuplesJsonFilePath = this.CreateProjectFileTuples(
                        datedOutputDirectoryPath,
                        projectsListTextFilePath,
                        buildProblemProjectsFilePath);

                    var (processingProblemsFilePath, processingProblemProjectsFilePath, instancesJsonFilePath) = await this.ProcessBuiltProjects(
                        datedOutputDirectoryPath,
                        logger);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var processingSummaryFilePath = this.SummarizeProcessing(
                        projectsListTextFilePath,
                        buildProblemProjectsFilePath,
                        processingProblemProjectsFilePath,
                        instancesJsonFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var (newInstancesJsonFilePath, oldInstancesJsonFilePath) = this.CompareDates(
                        instancesJsonFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var dateComparisonSummaryTextFilePath = this.SummarizeDatesComparison(
                        instancesJsonFilePath,
                        newInstancesJsonFilePath,
                        oldInstancesJsonFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var newAndOldSummaryTextFilePath = this.SummarizeNewAndOldInstances(
                        newInstancesJsonFilePath,
                        oldInstancesJsonFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    this.SendResultsEmail(
                        newAndOldSummaryTextFilePath,
                        dateComparisonSummaryTextFilePath,
                        processingSummaryFilePath);

                    Instances.OutputOperations.OutputInstanceSpecificFiles(
                        instancesJsonFilePath);

                    this.CopyFilesToCloudSharedDirectory(date);
                });
        }

        public void CopyFilesToCloudSharedDirectory(
            DateTime date)
        {
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var instanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            foreach (var instanceVarietyName in instanceVarietyNames)
            {
                var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                var sourceFilePath = Instances.PathOperator.GetFilePath(
                    datedOutputDirectoryPath,
                    fileName);

                var destinationFilePath = Instances.PathOperator.GetFilePath(
                    Instances.DirectoryPaths.CloudSharedInstancesDirectoryPath,
                    fileName);

                Instances.FileSystemOperator.CopyFile(
                    sourceFilePath,
                    destinationFilePath);
            }
        }

        public void SendResultsEmail(
            string newAndOldSummaryFilePath,
            string dateComparisonSummaryFilePath,
            string processingSummaryFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var toAddresses = new[]
            {
                Instances.EmailAddresses.David_Gmail,
                //Instances.EmailAddresses.Vedika_Gmail,
            };

            var today = Instances.DateOperator.GetToday();

            var subject = $"Instances Summary {Instances.DateOperator.ToString_YYYY_MM_DD_Dashed(today)}";

            var newAndOldSummaryLines = Instances.FileOperator.ActuallyReadAllLines_Synchronous(newAndOldSummaryFilePath);
            var dateComparisonSummaryLines = Instances.FileOperator.ActuallyReadAllLines_Synchronous(dateComparisonSummaryFilePath);
            var processingSummaryLines = Instances.FileOperator.ActuallyReadAllLines_Synchronous(processingSummaryFilePath);

            var bodyLines = Instances.EnumerableOperator.Empty<string>()
                .AppendRange(dateComparisonSummaryLines)
                .Append(Instances.Strings.Empty)
                .AppendRange(newAndOldSummaryLines)
                .Append(Instances.Strings.Empty)
                .AppendRange(processingSummaryLines)
                .Append($"\n\nSent by machine: {F0000.MachineNameOperator.Instance.GetMachineName()}");

            var body = Instances.StringOperator.Join(
                Environment.NewLine,
                bodyLines);

            var emailMessage = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            toAddresses.ForEach(toAddress => emailMessage.To.Add(new MailAddress(toAddress)));

            Instances.EmailSender.SendEmail(emailMessage);
        }

        public string SummarizeNewAndOldInstances(
            string newInstancesJsonFilePath,
            string oldInstancesJsonFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var newInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(newInstancesJsonFilePath);
            var oldInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(oldInstancesJsonFilePath);

            var varietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            var newInstanceNamesByVarietyName = newInstances
                .GroupBy(x => x.InstanceVariety)
                .OrderByNames(x => x.Key, varietyNames)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(x => x.ParameterNamedIdentityName).OrderAlphabetically().ToArray());

            var oldInstanceNamesByVarietyName = oldInstances
                .GroupBy(x => x.InstanceVariety)
                .OrderByNames(x => x.Key, varietyNames)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(x => x.ParameterNamedIdentityName).OrderAlphabetically().ToArray());

            var addedLine = newInstanceNamesByVarietyName.Any()
                ? Instances.Strings.Empty
                : " <none>"
                ;

            var removedLine = oldInstanceNamesByVarietyName.Any()
                ? Instances.Strings.Empty
                : " <none>"
                ;

            static IEnumerable<string> GetVarietyLines(KeyValuePair<string, string[]> xPair)
            {
                var lines = Instances.EnumerableOperator.From($"{xPair.Key} ({xPair.Value.Length}):")
                    .AppendRange(xPair.Value
                        .Select(x => $"\t{x}"))
                    .Append(Instances.Strings.Empty)
                    ;

                return lines;
            }

            var lines = Instances.EnumerableOperator.From("Changes:\n")
                .AppendRange(Instances.EnumerableOperator.From($"Added:{addedLine}\n")
                    .AppendRange(newInstanceNamesByVarietyName
                        .SelectMany(xPair => GetVarietyLines(xPair))))
                .AppendRange(Instances.EnumerableOperator.From($"Removed:{removedLine}\n")
                    .AppendRange(oldInstanceNamesByVarietyName
                        .SelectMany(xPair => GetVarietyLines(xPair))))
                ;

            var newAndOldSummaryTextFilePath = Instances.FilePathOperator.Get_NewAndOldSummaryTextFilePath(datedOutputDirectoryPath);

            Instances.FileOperator.WriteAllLines_Synchronous(
                newAndOldSummaryTextFilePath,
                lines);

            return newAndOldSummaryTextFilePath;
        }

        public string SummarizeDatesComparison(
            string instancesJsonFilePath,
            string newInstancesJsonFilePath,
            string oldInstancesJsonFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            var priorDate = this.GetPriorComparisonDate(date);


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(instancesJsonFilePath);
            var newInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(newInstancesJsonFilePath);
            var oldInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(oldInstancesJsonFilePath);

            var varietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            Dictionary<string, int> GetCountsByVarietyNames(
                IEnumerable<string> varietyNames,
                IEnumerable<InstanceDescriptor> instances)
            {
                var instanceCountsByVarietyName = instances
                    .GroupBy(x => x.InstanceVariety)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Count());

                var output = varietyNames
                    .Select(varietyName =>
                    {
                        var count = instanceCountsByVarietyName.ContainsKey(varietyName)
                            ? instanceCountsByVarietyName[varietyName]
                            : 0
                            ;

                        return (varietyName, count);
                    })
                    .ToDictionary(
                        x => x.varietyName,
                        x => x.count);

                return output;
            }

            Dictionary<string, (int instanceCount, int newInstanceCount, int oldInstanceCount)> GetAllCountsByVarietyNames(
                IEnumerable<string> varietyNames,
                IEnumerable<InstanceDescriptor> instances,
                IEnumerable<InstanceDescriptor> newInstances,
                IEnumerable<InstanceDescriptor> oldInstances)
            {
                var instanceCounts = GetCountsByVarietyNames(
                    varietyNames,
                    instances);

                var newInstanceCounts = GetCountsByVarietyNames(
                    varietyNames,
                    newInstances);

                var oldInstanceCounts = GetCountsByVarietyNames(
                    varietyNames,
                    oldInstances);

                var output = varietyNames
                    .ToDictionary(
                        x => x,
                        x => (instanceCounts[x], newInstanceCounts[x], oldInstanceCounts[x]));

                return output;
            }

            var allCountsByVarietyName = GetAllCountsByVarietyNames(
                varietyNames,
                instances,
                newInstances,
                oldInstances);

            var lines =
                new[]
                {
                    "Instances Summary",
                    $"\n{Instances.DateOperator.ToString_YYYYMMDD(date)}: as-of date",
                    $"{Instances.DateOperator.ToString_YYYYMMDD(priorDate)}: prior comparison date",
                    "",
                }
                .Append(varietyNames
                    .SelectMany(x =>
                    {
                        var (instanceCount, newInstanceCount, oldInstanceCount) = allCountsByVarietyName[x];

                        var output = new[]
                        {
                            $"{instanceCount,5}: {x}, (+{newInstanceCount}, -{oldInstanceCount})"
                        };

                        return output;
                    }));

            var dateComparisonSummaryTextFilePath = Instances.FilePathOperator.Get_DateComparisonSummaryTextFilePath(datedOutputDirectoryPath);

            Instances.FileOperator.WriteAllLines_Synchronous(
                dateComparisonSummaryTextFilePath,
                lines);

            return dateComparisonSummaryTextFilePath;
        }

        public (string newInstancesJsonFilePath, string oldInstancesJsonFilePath) CompareDates(
            string datedInstancesJsonFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            var priorDate = this.GetPriorComparisonDate(date);


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var priorDatedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(priorDate);

            var priorDatedInstancesJsonFilePath = Instances.FilePathOperator.Get_InstancesJsonFilePath(priorDatedOutputDirectoryPath);

            var datedInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(datedInstancesJsonFilePath);
            var priorDatedInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(priorDatedInstancesJsonFilePath);

            var newInstances = datedInstances.Except(
                priorDatedInstances,
                InstanceDescriptorEqualityComparer.Instance)
                .Now();

            var oldInstances = priorDatedInstances.Except(
                datedInstances,
                InstanceDescriptorEqualityComparer.Instance)
                .Now();

            var newInstancesJsonFilePath = Instances.FilePathOperator.Get_NewInstancesJsonFilePath(datedOutputDirectoryPath);
            var oldInstancesJsonFilePath = Instances.FilePathOperator.Get_OldInstancesJsonFilePath(datedOutputDirectoryPath);

            Instances.JsonOperator.Serialize(
                newInstancesJsonFilePath,
                newInstances);

            Instances.JsonOperator.Serialize(
                oldInstancesJsonFilePath,
                oldInstances);

            return (newInstancesJsonFilePath, oldInstancesJsonFilePath);
        }

        public string SummarizeProcessing(
            string projectsListTextFilePath,
            string buildProblemProjectsFilePath,
            string processingProblemProjectsFilePath,
            string instancesJsonFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);
            var buildProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(buildProblemProjectsFilePath);
            var processingProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(processingProblemProjectsFilePath);

            var builtProjectFilePaths = projectFilePaths.Except(buildProblemProjectFilePaths).Now();
            var processedProjectFilePaths = builtProjectFilePaths.Except(processingProblemProjectFilePaths).Now();

            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(instancesJsonFilePath);

            var now = Instances.NowOperator.GetNow();

            var summaryLines = EnumerableOperator.Instance.From($"{DateOperator.Instance.ToString_YYYYMMDD_HHMMSS(now)}: As-of")
                .Append(
                    $"({projectFilePaths.Length} / {builtProjectFilePaths.Length} / {processedProjectFilePaths.Length}): Projects, built projects, processed projects",
                    $"{instances.Length}: total instances");

            var processingSummaryFilePath = Instances.FilePathOperator.Get_ProcessingSummaryTextFilePath(datedOutputDirectoryPath);

            Instances.FileOperator.WriteLines_Synchronous(
                processingSummaryFilePath,
                summaryLines);

            return processingSummaryFilePath;
        }

        public async Task<(
            string processingProblemsFilePath,
            string processingProblemProjectsFilePath,
            string instancesJsonFilePath)>
            ProcessBuiltProjects(
            string datedOutputDirectoryPath,
            ILogger logger)
        {
            /// Run.
            var projectFileTuplesJsonFilePath = Instances.FilePathOperator.Get_ProjectFileTuplesJsonFilePath(datedOutputDirectoryPath);

            var projectFileTuples = Instances.JsonOperator.Deserialize_Synchronous<ProjectFileTuple[]>(
                projectFileTuplesJsonFilePath);

            var processingProblemsFilePath = Instances.FilePathOperator.Get_ProcessingProblemsTextFilePath(datedOutputDirectoryPath);
            var processingProblemProjectsFilePath = Instances.FilePathOperator.Get_ProcessingProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var instancesJsonFilePath = Instances.FilePathOperator.Get_InstancesJsonFilePath(datedOutputDirectoryPath);

            await Instances.TextOutputOperator.InTextOutputContext(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(BuildProjectFilePaths),
                Instances.LogFilePathOperator.GetLogFilePath,
                async textOutput =>
                {
                    await this.ProcessBuiltProjects(
                        projectFileTuples,
                        processingProblemsFilePath,
                        processingProblemProjectsFilePath,
                        instancesJsonFilePath,
                        textOutput);
                });

            return (
                processingProblemsFilePath,
                processingProblemProjectsFilePath,
                instancesJsonFilePath);
        }

        public DateTime GetPriorComparisonDate(DateTime date)
        {
            var outputDirectoryPath = Instances.DirectoryPathOperator.Get_ApplicationOutputDirectoryPath();
                

            // Look through the names of directories in the output directory.
            var directoryPaths = Instances.FileSystemOperator.EnumerateAllChildDirectoryPaths(outputDirectoryPath);

            // Find directories whose names are YYYYMMDDs,
            var directoryDates = directoryPaths
                .Select(directoryPath =>
                {
                    var directoryName = Instances.PathOperator.GetDirectoryNameOfDirectoryPath(directoryPath);

                    var isYYYYMMDD = DateOperator.Instance.IsYYYYMMDD(directoryName);
                    return isYYYYMMDD;
                })
                .Where(x => x.Exists)
                // Only prior dates.
                .Where(x => x.Result < date)
                .Select(x => x.Result)
                .Now();

            // If none, return the default comparison date. Note: the date-to-date survey comparision has been implemented such that if a dated file path does not exist, an empty set of descriptors is returned.
            if (directoryDates.None())
            {
                return SpecialDates.Instance.DefaultPriorComparisonDate;
            }

            // Sort those directories in reverse chronological order, and choose the most recent.
            var mostRecentDate = directoryDates
                .OrderReverseChronologically()
                .First();

            return mostRecentDate;
        }

        public string CreateProjectFileTuples(
            string datedOutputDirectoryPath,
            string projectsListTextFilePath,
            string buildProblemProjectsFilePath)
        {
            /// Run.
            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);
            var buildProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(buildProblemProjectsFilePath);

            var buildSuccessProjectFilePaths = projectFilePaths
                .Except(buildProblemProjectFilePaths)
                .Now();

            var projectFileTuples = this.CreateProjectFilesTuples(buildSuccessProjectFilePaths);

            // Write project file tuples file.
            var projectFileTuplesJsonFilePath = Instances.FilePathOperator.Get_ProjectFileTuplesJsonFilePath(datedOutputDirectoryPath);

            Instances.JsonOperator.Serialize(
                projectFileTuplesJsonFilePath,
                projectFileTuples);

            return projectFileTuplesJsonFilePath;
        }

        public async Task<(string buildProblemsFilePath, string buildProblemProjectsFilePath)> BuildProjectFilePaths(
            string datedOutputDirectoryPath,
            ILogger logger,
            bool rebuildFailedBuildsToCollectErrors,
            string projectsListTextFilePath)
        {
            var buildProblemsFilePath = Instances.FilePathOperator.Get_BuildProblemsTextFilePath(datedOutputDirectoryPath);
            var buildProblemProjectsFilePath = Instances.FilePathOperator.Get_BuildProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);

            await Instances.TextOutputOperator.InTextOutputContext(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(BuildProjectFilePaths),
                Instances.LogFilePathOperator.GetLogFilePath,
                async textOutput =>
                {
                    await this.BuildProjectFilePaths(
                        rebuildFailedBuildsToCollectErrors,
                        projectFilePaths,
                        buildProblemsFilePath,
                        buildProblemProjectsFilePath,
                        textOutput);
                });

            return (buildProblemsFilePath, buildProblemProjectsFilePath);
        }

        ///// <inheritdoc cref="F001.Documentation."/>
        /// <summary>
        /// Also removes project files that should not be built.
        /// </summary>
        /// <returns>The projects list text file path.</returns>
        public string GetAllProjectFilePaths(
            string datedOutputDirectoryPath,
            ILogger logger)
        {
            // Output project paths to current run date's directory.
            var projectsListTextFilePath = Instances.FilePathOperator.Get_ProjectsListTextFilePath(
                datedOutputDirectoryPath);

            Instances.TextOutputOperator.InTextOutputContext_Synchronous(
                Instances.HumanOutputTextFilePathOperator.GetHumanOutputTextFilePath,
                nameof(BuildProjectFilePaths),
                Instances.LogFilePathOperator.GetLogFilePath,
                textOutput =>
                {
                    var projectsList_AllTextFilePath = Instances.FilePathOperator.Get_ProjectsList_AllTextFilePath(
                        datedOutputDirectoryPath);

                    this.Find_AllProjectFilePaths(
                        projectsList_AllTextFilePath,
                        projectsListTextFilePath,
                        Instances.FilePaths.DoNotBuildProjectsListTextFilePath,
                        textOutput);
                });

            return projectsListTextFilePath;
        }
    }
}
