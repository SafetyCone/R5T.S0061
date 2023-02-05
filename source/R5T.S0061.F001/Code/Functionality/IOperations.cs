using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging;

using System.Extensions;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker
    {
        /// <summary>
        /// If the runtime directory is just the .NET Core runtime directory, then it should not be included (false).
        /// Otherwise, if the runtime directory is the ASP.NET Core or the Windows Forms runtime directory, it should be included (true).
        /// </summary>
        public bool ShouldIncludeRuntimeDirectory(string runtimeDirectoryPath)
        {
            var output = runtimeDirectoryPath != Instances.RuntimeDirectoryPaths.NetCore;
            return output;
        }

        public string DetermineReflectionRuntimeDirectoryForProject(
            string projectFilePath)
        {
            var reflectionRuntimeDirectory = Instances.ProjectFileOperator.InQueryProjectFileContext_Synchronous(
                projectFilePath,
                projectElement =>
                {
                    // Is the project a web project?
                    var sdk = Instances.ProjectXmlOperator.GetSdk(projectElement);

                    var isWebProject = Instances.ProjectSdkStringOperations.Is_WebSdk(sdk);
                    if(isWebProject)
                    {
                        return Instances.RuntimeDirectoryPaths.AspNetCore;
                    }

                    // Is the project a windows forms project?
                    var targetframework = Instances.ProjectXmlOperator.GetTargetFramework(projectElement);
                    if(Instances.StringOperator.Contains(
                        targetframework,
                        "windows"))
                    {
                        return Instances.RuntimeDirectoryPaths.WindowsFormsNetCore;
                    }

                    // Else, just the regular .NET.
                    return Instances.RuntimeDirectoryPaths.NetCore;
                });

            return reflectionRuntimeDirectory;
        }

        public ProjectFilesTuple[] CreateProjectFilesTuples(
            IList<string> projectFilePaths)
        {
            var projectFileTuples = projectFilePaths
                .Select(this.CreateProjectFilesTuple)
                .Now();

            return projectFileTuples;
        }

        public ProjectFilesTuple CreateProjectFilesTuple(
            string projectFilePath)
        {
            var assemblyFilePath = Instances.FilePathOperator.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

            var documentationFilePath = Instances.ProjectPathsOperator.GetDocumentationFilePath_ForAssemblyFilePath(assemblyFilePath);

            var projectFilesTuple = new ProjectFilesTuple
            {
                ProjectFilePath = projectFilePath,
                AssemblyFilePath = assemblyFilePath,
                DocumentationFilePath = documentationFilePath,
            };

            return projectFilesTuple;
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

                Instances.AssemblyOperator.ForTypes(
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

        public InstanceDescriptor[] ProcessAssemblyFile(
            string projectFilePath,
            string assemblyFilePath,
            string documentationForAssemblyFilePath)
        {
            var instanceDescriptorsWithoutProjectFile = this.ProcessAssemblyFile_AddRuntimeDirectoryPaths(
                projectFilePath,
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

        public T001.N001.InstanceDescriptor[] ProcessAssemblyFile_AddRuntimeDirectoryPaths(
            string projectFilePath,
            string assemblyFilePath,
            string documentationForAssemblyFilePath)
        {
            var runtimeDirectoryPath = this.DetermineReflectionRuntimeDirectoryForProject(
                projectFilePath);

            var shouldIncludeRuntimeDirectoryPath = this.ShouldIncludeRuntimeDirectory(runtimeDirectoryPath);

            var executingRuntimeDirectoryPath = Instances.ReflectionOperator.GetExecutingRuntimeDirectoryPath();

            var runtimeDirectoryPaths = shouldIncludeRuntimeDirectoryPath
                ? Instances.EnumerableOperator.From(
                    executingRuntimeDirectoryPath,
                    runtimeDirectoryPath)
                : Instances.EnumerableOperator.From(executingRuntimeDirectoryPath)
                ;

            var output = this.ProcessAssemblyFile_SpecifyRuntimeDirectoryPaths(
                assemblyFilePath,
                documentationForAssemblyFilePath,
                runtimeDirectoryPaths);

            return output;
        }

        public T001.N001.InstanceDescriptor[] ProcessAssemblyFile_SpecifyRuntimeDirectoryPath(
            string assemblyFilePath,
            string documentationForAssemblyFilePath,
            string runtimeDirectoryPath)
        {
            var documentationByMemberIdentityName = Instances.DocumentationOperations.GetDocumentationByMemberIdentityName(
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
                    //});
                    //},
                    //// For now, always include the ASP.NET Core runtime.
                    //Instances.RuntimeDirectoryPaths.AspNetCore);
                },
                runtimeDirectoryPath);

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

        public T001.N001.InstanceDescriptor[] ProcessAssemblyFile_SpecifyRuntimeDirectoryPaths(
            string assemblyFilePath,
            string documentationForAssemblyFilePath,
            IEnumerable<string> runtimeDirectoryPaths)
        {
            var documentationByMemberIdentityName = Instances.DocumentationOperations.GetDocumentationByMemberIdentityName(
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
                    //});
                    //},
                    //// For now, always include the ASP.NET Core runtime.
                    //Instances.RuntimeDirectoryPaths.AspNetCore);
                },
                runtimeDirectoryPaths);

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

        public void ProcessBuiltProjects(
            IList<ProjectFilesTuple> projectFileTuples,
            string processingProblemsTextFilePath,
            string processingProblemProjectsTextFilePath,
            string instancesJsonFilePath,
            ILogger logger)
        {
            var (processingProblemTextsByProjectFilePath, instances) = this.ProcessBuiltProjects(
                projectFileTuples,
                logger);

            this.WriteProblemProjectsFile(
                processingProblemsTextFilePath,
                processingProblemTextsByProjectFilePath);

            Instances.FileOperator.WriteLines_Synchronous(
                processingProblemProjectsTextFilePath,
                processingProblemTextsByProjectFilePath.Keys
                    .OrderAlphabetically());

            Instances.JsonOperator.Serialize(
               instancesJsonFilePath,
               instances);
        }

        public (
            Dictionary<string, string> processingProblemTextsByProjectFilePath,
            InstanceDescriptor[] instances)
            ProcessBuiltProjects(
            IList<ProjectFilesTuple> projectFileTuples,
            ILogger logger)
        {
            var instances = new List<InstanceDescriptor>();

            var processingProblemTextsByProjectFilePath = new Dictionary<string, string>();

            var projectCounter = 1; // Start at 1.
            var projectCount = projectFileTuples.Count;

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

            return (
                processingProblemTextsByProjectFilePath,
                instances.ToArray());
        }

        public Dictionary<string, string> BuildProjectFilePaths(
            bool rebuildFailedBuildsToCollectErrors,
            IList<string> projectFilePaths,
            string buildProblemsFilePath,
            string buildProblemProjectsFilePath,
            ILogger logger)
        {
            var buildProblemTextsByProjectFilePath = new Dictionary<string, string>();

            var projectCounter = 1; // Start at 1.
            var projectCount = projectFilePaths.Count;

            foreach (var projectFilePath in projectFilePaths)
            {
                logger.LogInformation("Building project ({projectCounter} / {projectCount}):\n\t{projectFilePath}",
                    projectCounter++,
                    projectCount,
                    projectFilePath);

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

            return buildProblemTextsByProjectFilePath;
        }

        /// <summary>
        /// Builds the project with the win-x64 runtime argument to get a self-contained executable.
        /// (To allow full assembly reflection.)
        /// </summary>
        public void BuildProject(
            string projectFilePath,
            ILogger logger)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            Instances.FileSystemOperator.ClearDirectory_Synchronous(publishDirectoryPath);

            // Publishing as a self-contained application solves the issue with framework assemblies not being available (for example, ASP.NET Core framework assemblies not being available from .NET Core).
            // But! At the cost of tremendous hard disk space (~100MB for each target).
            //Instances.DotnetPublishOperator.Publish_WithRuntimeArgument(
            //    projectFilePath,
            //    publishDirectoryPath);

            // Instead, use the regular publish operation for HD space.
            // Not sure what to do in the reflection step. I have tried:
            // 1. Adding a framework reference in the CSPROJ file of the executable doing the reflection.
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
                var buildProblemText = Instances.TextOperator.JoinLines(
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
        public void GetAllProjectFilePaths(
            string outputTextFilePath,
            ILogger logger)
        {
            // Output project paths to current run date's directory.
            var repositoriesDirectoryPaths = Instances.RepositoriesDirectoryPaths.AllOfMine;

            var projectFilePaths = Instances.FileSystemOperator.GetAllProjectFilePaths_FromRepositoriesDirectoryPaths(
                repositoriesDirectoryPaths,
                logger)
                .OrderAlphabetically()
                .Now();

            Instances.FileOperator.WriteLines_Synchronous(
                outputTextFilePath,
                projectFilePaths);
        }

        public void WriteProblemProjectsFile(
            string problemProjectsFilePath,
            Dictionary<string, string> problemProjects)
        {
            Instances.FileOperator.WriteAllLines_Synchronous(
                problemProjectsFilePath,
                Instances.EnumerableOperator.From($"Problem Projects, Count: {problemProjects.Count}\n\n")
                    .Append(problemProjects
                        .OrderAlphabetically(pair => pair.Key)
                        .SelectMany(pair => Instances.EnumerableOperator.From($"{pair.Key}:")
                            .Append(pair.Value)
                            .Append("***\n"))));
        }
    }
}
