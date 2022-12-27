using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Xml.XPath;

using Microsoft.Extensions.Logging;

using System.Extensions;

using R5T.F0000;
using R5T.T0132;

using R5T.S0061.T001;
using System.Threading;

namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker
    {
        public void Run()
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();
            // True, if you want to spend the time to rebuild failed builds in order to collect build errors during this run.
            var rebuildFailedBuildsToCollectErrors = true;


            /// Run.
            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                Instances.Values.ApplicationName,
                logger =>
                {
                    var projectsListTextFilePath = this.GetAllProjectFilePaths(
                        date,
                        logger);

                    var (buildProblemsFilePath, buildProblemProjectsFilePath) = this.BuildProjectFilePaths(
                        date,
                        logger,
                        rebuildFailedBuildsToCollectErrors,
                        projectsListTextFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var projectFileTuplesJsonFilePath = this.CreateProjectFileTuples(
                        date,
                        projectsListTextFilePath,
                        buildProblemProjectsFilePath);

                    // Wait a second for file handles to be released.
                    Thread.Sleep(1000);

                    var (processingProblemsFilePath, processingProblemProjectsFilePath, instancesJsonFilePath) = this.ProcessBuiltProjects(
                        date);

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

                    this.OutputInstanceSpecificFiles(
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

        public void OutputInstanceSpecificFiles(
            string instancesJsonFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.GetToday();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(instancesJsonFilePath);
            var instancesByVariety = instances
                .GroupBy(x => x.InstanceVariety)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());

            var instanceVarietyNames = Instances.InstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            foreach (var instanceVarietyName in instanceVarietyNames)
            {
                var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                var outputFilePath = Instances.PathOperator.GetFilePath(
                    datedOutputDirectoryPath,
                    fileName);

                var instancesOfVariety = instancesByVariety.ContainsKey(instanceVarietyName)
                    ? instancesByVariety[instanceVarietyName]
                    : Array.Empty<InstanceDescriptor>()
                    ;

                var title = instanceVarietyName;

                var lines = Instances.EnumerableOperator.From($"{title}, Count: {instancesOfVariety.Length}\n\n")
                    .Append(instancesOfVariety
                        .GroupBy(x => x.ProjectFilePath)
                        .OrderAlphabetically(x => x.Key)
                        .SelectMany(xGroup => Instances.EnumerableOperator.From($"{xGroup.Key}:")
                            .Append(xGroup
                                // Order by the identity name.
                                .OrderAlphabetically(x => x.IdentityName)
                                // But output the parameter named identity name.
                                .Select(x => $"\t{x.ParameterNamedIdentityName}")
                                .Append(Instances.Strings.Empty))));

                Instances.FileOperator.WriteAllLines_Synchronous(
                    outputFilePath,
                    lines);
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
                Instances.EmailAddresses.Vedika_Gmail,
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

        public (string processingProblemsFilePath,
            string processingProblemProjectsFilePath,
            string instancesJsonFilePath)
            ProcessBuiltProjects(
            DateTime date)
        {
            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var projectFileTuplesJsonFilePath = Instances.FilePathOperator.Get_ProjectFileTuplesJsonFilePath(datedOutputDirectoryPath);

            var projectFileTuples = Instances.JsonOperator.Deserialize_Synchronous<ProjectFilesTuple[]>(
                projectFileTuplesJsonFilePath);

            var processingProblemsFilePath = Instances.FilePathOperator.Get_ProcessingProblemsTextFilePath(datedOutputDirectoryPath);
            var processingProblemProjectsFilePath = Instances.FilePathOperator.Get_ProcessingProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var instancesJsonFilePath = Instances.FilePathOperator.Get_InstancesJsonFilePath(datedOutputDirectoryPath);

            Instances.LoggingOperator.InConsoleLoggerContext_Synchronous(
                nameof(ProcessBuiltProjects),
                logger =>
                {
                    var instances = new List<InstanceDescriptor>();

                    var processingProblemTextsByProjectFilePath = new Dictionary<string, string>();

                    var projectCounter = 1; // Start at 1.
                    var projectCount = projectFileTuples.Length;

                    foreach (var tuple in projectFileTuples)
                    {
                        logger.LogInformation("Processing project ({projectCounter} / {projectCount}):\n\t{projectFile}",
                            projectCounter++,
                            projectCount,
                            tuple.ProjectFilePath);

                        var assemblyFileExists = Instances.FileSystemOperator.FileExists(
                                tuple.AssemblyFilePath);

                        if (!assemblyFileExists)
                        {
                            processingProblemTextsByProjectFilePath.Add(
                                tuple.ProjectFilePath,
                                "No assembly file.");

                            logger.LogInformation("No assembly file to process, build failed.");

                            continue;
                        }

                        try
                        {
                            var currentInstances = Operations.Instance.ProcessAssemblyFile(
                                tuple.ProjectFilePath,
                                tuple.AssemblyFilePath,
                                tuple.DocumentationFilePath);

                            instances.AddRange(currentInstances);

                            logger.LogInformation("Processed project.");
                        }
                        catch (Exception ex)
                        {
                            processingProblemTextsByProjectFilePath.Add(
                                tuple.ProjectFilePath,
                                ex.Message);
                        }
                    }

                    this.WriteProblemProjectsFile(
                        processingProblemsFilePath,
                        processingProblemTextsByProjectFilePath);

                    Instances.FileOperator.WriteLines_Synchronous(
                        processingProblemProjectsFilePath,
                        processingProblemTextsByProjectFilePath.Keys
                            .OrderAlphabetically());

                    Instances.JsonOperator.Serialize(
                       instancesJsonFilePath,
                       instances);
                });

            return (
                processingProblemsFilePath,
                processingProblemProjectsFilePath,
                instancesJsonFilePath);
        }

        public InstanceDescriptor[] ProcessAssemblyFile(
            string projectFilePath,
            string assemblyFilePath,
            string documentationForAssemblyFilePath)
        {
            var instanceDescriptorsWithoutProjectFile = this.ProcessAssemblyFile(
                assemblyFilePath,
                documentationForAssemblyFilePath);

            var output = instanceDescriptorsWithoutProjectFile
                .Select(x =>
                {
                    var instanceDescriptor = new InstanceDescriptor
                    {
                        ProjectFilePath = projectFilePath,
                        InstanceVariety = x.InstanceVariety,
                        IdentityName = x.IdentityName,
                        ParameterNamedIdentityName = x.ParameterNamedIdentityName,
                        DescriptionXml = x.DescriptionXml,
                    };

                    return instanceDescriptor;
                })
                .Now();

            return output;
        }

        public T001.N001.InstanceDescriptor[] ProcessAssemblyFile(
            string assemblyFilePath,
            string documentationForAssemblyFilePath)
        {
            var documentationByMemberIdentityName = Operations.Instance.GetDocumentationByMemberIdentityName(
                documentationForAssemblyFilePath);

            var intanceIdentityNamesProvidersByInstanceVariety = Operations.Instance.GetInstanceIdentityNamesProvidersByInstanceVariety();

            var instanceIdentityNameSetsByVarietyType = Instances.ReflectionOperator.InAssemblyContext(
                assemblyFilePath,
                assembly =>
                {
                    var output = intanceIdentityNamesProvidersByInstanceVariety
                        .Select(pair =>
                        {
                            var instanceIdentityNamesSet = pair.Value(assembly);

                            return (pair.Key, instanceIdentityNamesSet);
                        })
                        .ToDictionary(
                            x => x.Key,
                            x => x.instanceIdentityNamesSet);

                    return output;
                });

            var output = instanceIdentityNameSetsByVarietyType
                .SelectMany(pair =>
                {
                    var instanceVariety = pair.Key;

                    var output = pair.Value
                        .Select(x =>
                        {
                            var documentationXml = documentationByMemberIdentityName.HasValue(x.IdentityName)
                                ? documentationByMemberIdentityName[x.IdentityName]
                                : default
                                ;

                            var output = new T001.N001.InstanceDescriptor
                            {
                                InstanceVariety = instanceVariety,
                                IdentityName = x.IdentityName,
                                ParameterNamedIdentityName = x.ParameterNamedIdentityName,
                                DescriptionXml = documentationXml,
                            };

                            return output;
                        });

                    return output;
                })
                .Now();

            return output;
        }

        public Dictionary<string, string> GetDocumentationByMemberIdentityName(
            string documentationXmlFilePath)
        {
            var output = new Dictionary<string, string>();

            var documentationFileExists = Instances.FileSystemOperator.FileExists(documentationXmlFilePath);
            if (documentationFileExists)
            {
                var documentation = XmlOperator.Instance.Load_XDocument(documentationXmlFilePath);

                var membersNode = documentation.XPathSelectElement("//doc/members");

                var membersNodeExists = membersNode is not null;
                if (membersNodeExists)
                {
                    var memberNodes = membersNode.XPathSelectElements("member");
                    foreach (var memberNode in memberNodes)
                    {
                        var memberIdentityName = memberNode.Attribute("name").Value;
                        var documentationForMember = memberNode.FirstNode.ToString();

                        var prettyPrintedDocumentationForMember = DocumentationOperator.Instance.PrettyPrint(documentationForMember);

                        output.Add(memberIdentityName, prettyPrintedDocumentationForMember);
                    }
                }
            }

            return output;
        }

        public Dictionary<string, Func<Assembly, InstanceIdentityNames[]>> GetInstanceIdentityNamesProvidersByInstanceVariety()
        {
            // Type name data of types for which we want method names.
            var methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = Values.Instance.MethodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety;

            // Type name data of types for which we want property names.
            var propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety = Values.Instance.PropertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety;

            // Type name data of types for which we want type names.
            var instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName = Values.Instance.InstanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName;

            // Build the closures that will perform Assembly => InstancesIdentityNames for each type of code element (method or property), for each variety of instance (functionality, explorations, etc.).
            var intanceIdentityNamesProvidersByInstanceVariety = methodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, this.GetInstanceMethodNamesProviderFunction(xPair.Value)))
                .Append(propertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety
                    .Select(xPair => (xPair.Key, this.GetInstancePropertyNamesProviderFunction(xPair.Value))))
                .Append(instanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName
                    .Select(xPair => (xPair.Key, this.GetInstanceTypeNamesProviderFunction(xPair.Value))))
                //// For debugging.
                //.Where(x => x.Key == Instances.InstanceVariety.MarkerAttribute)
                ////
                .ToDictionary(
                    x => x.Key,
                    x => x.Item2);

            return intanceIdentityNamesProvidersByInstanceVariety;
        }

        public Func<Assembly, InstanceIdentityNames[]> GetInstanceTypeNamesProviderFunction(
            string markerAttributeNamespacedTypeName)
        {
            InstanceIdentityNames[] Internal(Assembly assembly)
            {
                var functionalityMethodNamesSet = new List<InstanceIdentityNames>();

                AssemblyOperator.Instance.ForTypes(
                    assembly,
                    Instances.ReflectionOperations.GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName),
                    typeInfo =>
                    {
                        var functionalityMethodNames = Instances.ReflectionOperations.GetTypeInstanceIdentityNames(typeInfo);

                        functionalityMethodNamesSet.Add(functionalityMethodNames);
                    });

                return functionalityMethodNamesSet.ToArray();
            }

            return Internal;
        }

        public Func<Assembly, InstanceIdentityNames[]> GetInstancePropertyNamesProviderFunction(
            string markerAttributeNamespacedTypeName)
        {
            InstanceIdentityNames[] Internal(Assembly assembly)
            {
                var functionalityMethodNamesSet = new List<InstanceIdentityNames>();

                Instances.ReflectionOperations.ForPropertiesOnTypes(
                    assembly,
                    Instances.ReflectionOperations.GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName),
                    Instances.ReflectionOperations.IsValuesProperty,
                    (typeInfo, propertyInfo) =>
                    {
                        var functionalityMethodNames = Instances.ReflectionOperations.GetValuePropertyNames(propertyInfo);

                        functionalityMethodNamesSet.Add(functionalityMethodNames);
                    });

                return functionalityMethodNamesSet.ToArray();
            }

            return Internal;
        }

        

        public Func<Assembly, InstanceIdentityNames[]> GetInstanceMethodNamesProviderFunction(
            string markerAttributeNamespacedTypeName)
        {
            InstanceIdentityNames[] Internal(Assembly assembly)
            {
                var typeInstanceIdentityNamesSet = new List<InstanceIdentityNames>();

                Instances.ReflectionOperations.ForMethodsOnTypes(
                    assembly,
                    Instances.ReflectionOperations.GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName),
                    Instances.ReflectionOperations.IsInstanceMethod,
                    (typeInfo, methodInfo) =>
                    {
                        var functionalityMethodNames = this.GetMethodInstanceIdentityNames(methodInfo);

                        typeInstanceIdentityNamesSet.Add(functionalityMethodNames);
                    });

                return typeInstanceIdentityNamesSet.ToArray();
            }

            return Internal;
        }

        public InstanceIdentityNames GetMethodInstanceIdentityNames(MethodInfo methodInfo)
        {
            var methodIdentityName = Instances.IdentityNameProvider.GetIdentityName(methodInfo);

            var methodParameterNamedIdentityName = Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(methodInfo);

            var output = new InstanceIdentityNames
            {
                IdentityName = methodIdentityName,
                ParameterNamedIdentityName = methodParameterNamedIdentityName,
            };

            return output;
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
            DateTime date,
            string projectsListTextFilePath,
            string buildProblemProjectsFilePath)
        {
            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);
            var buildProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(buildProblemProjectsFilePath);

            var buildSuccessProjectFilePaths = projectFilePaths
                .Except(buildProblemProjectFilePaths)
                .Now();

            var projectFileTuples = buildSuccessProjectFilePaths
                .Select(projectFilePath =>
                {
                    var assemblyFilePath = Instances.FilePathOperator.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

                    var documentationFilePath = F0040.ProjectPathsOperator.Instance.GetDocumentationFilePath_ForAssemblyFilePath(assemblyFilePath);

                    var projectFilesTuple = new ProjectFilesTuple
                    {
                        ProjectFilePath = projectFilePath,
                        AssemblyFilePath = assemblyFilePath,
                        DocumentationFilePath = documentationFilePath,
                    };

                    return projectFilesTuple;
                })
                .Now();

            // Write project file tuples file.
            var projectFileTuplesJsonFilePath = Instances.FilePathOperator.Get_ProjectFileTuplesJsonFilePath(datedOutputDirectoryPath);

            Instances.JsonOperator.Serialize(
                projectFileTuplesJsonFilePath,
                projectFileTuples);

            return projectFileTuplesJsonFilePath;
        }

        public (string buildProblemsFilePath, string buildProblemProjectsFilePath) BuildProjectFilePaths(
            DateTime date,
            ILogger logger,
            bool rebuildFailedBuildsToCollectErrors,
            string projectsListTextFilePath)
        {
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var buildProblemsFilePath = Instances.FilePathOperator.Get_BuildProblemsTextFilePath(datedOutputDirectoryPath);
            var buildProblemProjectsFilePath = Instances.FilePathOperator.Get_BuildProblemProjectsTextFilePath(datedOutputDirectoryPath);

            var projectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(projectsListTextFilePath);

            var buildProblemTextsByProjectFilePath = new Dictionary<string, string>();

            var projectCounter = 1; // Start at 1.
            var projectCount = projectFilePaths.Length;

            foreach (var projectFilePath in projectFilePaths)
            {
                logger.LogInformation("Building project ({projectCounter} / {projectCount}):\n\t{projectFilePath}",
                    projectCounter++,
                    projectCount,
                    projectFilePath);

                var projectDirectoryPath = Instances.ProjectPathsOperator.GetProjectDirectoryPath(projectFilePath);

                var shouldBuildProject = this.ShouldBuildProject(
                    projectFilePath,
                    rebuildFailedBuildsToCollectErrors,
                    logger);

                if (shouldBuildProject)
                {
                    // Clear the publish directory and publish (build), and not any problems.
                    this.BuildProject(
                        projectFilePath,
                        logger,
                        buildProblemTextsByProjectFilePath);
                }
                else
                {
                    // See if a prior attempt to build the project failed, and not the failure.
                    var hasOutputAssembly = Instances.FileSystemOperator.Has_OutputAssembly(projectFilePath);
                    if (!hasOutputAssembly)
                    {
                        var buildProblemText = "Prior builds failed, and option to rebuild prior failed builds to collect errors was not true.";

                        buildProblemTextsByProjectFilePath.Add(
                            projectFilePath,
                            buildProblemText);
                    }
                }
            }

            // Write build problems file.
            this.WriteProblemProjectsFile(
                buildProblemsFilePath,
                buildProblemTextsByProjectFilePath);

            // Write build problem projects file.
            Instances.FileOperator.WriteLines(
                buildProblemProjectsFilePath,
                buildProblemTextsByProjectFilePath.Keys
                    .OrderAlphabetically());

            return (buildProblemsFilePath, buildProblemProjectsFilePath);
        }

        public void BuildProject(
            string projectFilePath,
            ILogger logger)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            Instances.FileSystemOperator.ClearDirectory_Synchronous(publishDirectoryPath);

            Instances.DotnetPublishOperator.Publish(
                projectFilePath,
                publishDirectoryPath);

            logger.LogInformation("Built project.");
        }

        public void BuildProject(
            string projectFilePath,
            ILogger logger,
            Dictionary<string, string> buildProblemTextsByProjectFilePath)
        {
            try
            {
                this.BuildProject(
                    projectFilePath,
                    logger);
            }
            catch (AggregateException aggregateException)
            {
                logger.LogWarning("Failed to build project.");

                // Combine aggregate exception messages to text.
                var buildProblemText = TextOperator.Instance.JoinLines(
                    aggregateException.InnerExceptions
                        .Select(exception => exception.Message));

                buildProblemTextsByProjectFilePath.Add(
                    projectFilePath,
                    buildProblemText);
            }

            // Output an S0041-specific file (R5T.S0041.Build.json) containing build time to publish directory.
            // Output this regardless of build success so that projects are not rebuilt in either case until project files change.
            var buildTime = Instances.NowOperator.GetNow_Local();

            var buildResult = new BuildResult
            {
                Timestamp = buildTime,
            };

            var buildJsonFilePath = Instances.FilePathOperator.Get_BuildJsonFilePath(projectFilePath);

            Instances.JsonOperator.Serialize_Synchronous(
                buildJsonFilePath,
                buildResult);
        }

        public void WriteProblemProjectsFile(
            string problemProjectsFilePath,
            Dictionary<string, string> problemProjects)
        {
            FileOperator.Instance.WriteAllLines_Synchronous(
                problemProjectsFilePath,
                EnumerableOperator.Instance.From($"Problem Projects, Count: {problemProjects.Count}\n\n")
                    .Append(problemProjects
                        .OrderAlphabetically(pair => pair.Key)
                        .SelectMany(pair => EnumerableOperator.Instance.From($"{pair.Key}:")
                            .Append(pair.Value)
                            .Append("***\n"))));
        }

        public bool ShouldBuildProject(
            string projectFilePath,
            bool rebuildFailedBuildsToCollectErrors,
            ILogger logger)
        {
            logger.LogInformation("Determining whether the project should be built:\n\t{projectFilePath}", projectFilePath);

            var neverBuiltBefore = this.ShouldBuildProject_NeverBuiltBefore(
                projectFilePath,
                logger);

            if (neverBuiltBefore)
            {
                logger.LogInformation("Build project: never built (as part of this process).");

                return true;
            }

            var anyChangesSinceLastBuild = this.ShouldBuildProject_AnyChangesSinceLastBuild(
                projectFilePath,
                logger);

            if (anyChangesSinceLastBuild)
            {
                logger.LogInformation("Build project: changes found since last build.");

                return true;
            }

            // At this point, we know *an attempt* to build project has been tried before, and that there were no changes since the last attempt.

            // If the output assembly was not found, we should re-build the project.
            var outputAssemblyNotFound = this.ShouldBuildProject_OutputAssemblyNotFound(
                projectFilePath,
                logger);

            // But only if we want to wait to rebuild projects for which prior build attempts have failed.
            var rebuildProjectAfterPriorFailedBuilds = outputAssemblyNotFound && rebuildFailedBuildsToCollectErrors;

            if (rebuildProjectAfterPriorFailedBuilds)
            {
                logger.LogInformation("Build project: rebuild project after prior failure.");

                return true;
            }

            logger.LogInformation("Do not build project.");

            return false;
        }

        /// <summary>
        /// If the project has never been built before (as part of this process), it should be built.
        /// </summary>
        public bool ShouldBuildProject_NeverBuiltBefore(
            string projectFilePath,
            ILogger logger)
        {
            // Determine whether the project has been built before as part of this process by testing for the existence of the output build file specific to this process.
            var hasBuildResultFile = Instances.FileSystemOperator.Has_BuildResultFile(projectFilePath);

            if (hasBuildResultFile)
            {
                logger.LogInformation("Should not build: already built (as part of this process).");
                return false;
            }
            else
            {
                logger.LogInformation("Should build: never built (as part of this process).");
                return true;
            }
        }

        /// <summary>
        /// If a project has not been built (built during a prior run of this process, then it should be built).
        /// </summary>
        public bool ShouldBuildProject_OutputAssemblyNotFound(
            string projectFilePath,
            ILogger logger)
        {
            var outputAssemblyExists = Instances.FileSystemOperator.Has_OutputAssembly(projectFilePath);
            if (outputAssemblyExists)
            {
                logger.LogInformation("Should not build: output assembly already exists.");
                return false;
            }
            else
            {
                logger.LogInformation("Should build: output assembly does not exist.");
                return true;
            }
        }

        /// <summary>
        /// If a project has not changed since the last time it was built, then it should not be built (re-built).
        /// For the specific application of determining instances within a project, we only need to rebuild a project if files within that project have changed.
        /// Note: but for the general case of determining whether a project should be rebuilt, an examination of all files in the full recursive project references hierarchy should be performed (even including NuGet package reference update rules evaluation).
        /// </summary>
        public bool ShouldBuildProject_AnyChangesSinceLastBuild(
            string projectFilePath,
            ILogger logger)
        {
            // Assume that a project should be built.
            var shouldBuildProject = true;

            // Check latest file write time in project directory against build time in publish directory (R5T.S0041.Build.json).
            var projectDirectoryPath = Instances.ProjectPathsOperator.GetProjectDirectoryPath(projectFilePath);

            var projectFilesLastModifiedTime = F0000.FileSystemOperator.Instance.GetLastModifiedTime_ForDirectory_Local(
                projectDirectoryPath,
                Instances.DirectoryNameOperator.IsNotBinariesOrObjectsDirectory);

            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var buildJsonFilePath = Instances.FilePathOperator.Get_BuildJsonFilePath_FromPublishDirectory(publishDirectoryPath);

            var buildJsonFileExists = Instances.FileSystemOperator.FileExists(buildJsonFilePath);
            if (buildJsonFileExists)
            {
                var buildResult = Instances.JsonOperator.Deserialize_Synchronous<BuildResult>(
                    buildJsonFilePath);

                var lastBuildTime = buildResult.Timestamp;

                // If the last build time is greater than latest file write time, skip building project.
                var skipRepublishProject = lastBuildTime > projectFilesLastModifiedTime;
                if (skipRepublishProject)
                {
                    logger.LogInformation("Skip building project. (Project files last modified time: {projectFilesLastModifiedTime}, last build time: {lastBuildTime}", projectFilesLastModifiedTime, lastBuildTime);

                    shouldBuildProject = false;
                }
            }

            return shouldBuildProject;
        }

        /// <summary>
        /// Searches the file-system in each of the repositories directory paths.
        /// </summary>
        /// <returns>The projects list text file path.</returns>
        public string GetAllProjectFilePaths(
            DateTime date,
            ILogger logger)
        {
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            // Output project paths to current run date's directory.
            var projectsListTextFilePath = Instances.FilePathOperator.Get_ProjectsListTextFilePath(
                datedOutputDirectoryPath);

            var repositoriesDirectoryPaths = Instances.RepositoriesDirectoryPaths.AllOfMine;

            var projectFilePaths = Instances.FileSystemOperator.GetAllProjectFilePaths_FromRepositoriesDirectoryPaths(
                repositoriesDirectoryPaths,
                logger)
                .OrderAlphabetically()
                .Now();

            Instances.FileOperator.WriteLines_Synchronous(
                projectsListTextFilePath,
                projectFilePaths);

            return projectsListTextFilePath;
        }
    }
}
