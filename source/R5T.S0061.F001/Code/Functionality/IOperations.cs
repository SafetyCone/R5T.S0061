using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using System.Extensions;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker
    {
        public bool ShouldIncludeAspNetRuntimeDirectory(
            string projectFilePath,
            XElement projectElement,
            ProjectDependenciesSet projectDependenciesSet)
        {
            var sdk = Instances.ProjectXmlOperator.GetSdk(projectElement);

            var isWebSdk = Instances.ProjectSdkStringOperations.Is_WebSdk(sdk);
            if(isWebSdk)
            {
                return true;
            }

            var hasAspNetDependency = projectDependenciesSet.HasAspNetDependencyByProjectFilePath[projectFilePath];
            if(hasAspNetDependency)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// If the runtime directory is just the .NET Core runtime directory, then it should not be included (false).
        /// Otherwise, if the runtime directory is the ASP.NET Core or the Windows Forms runtime directory, it should be included (true).
        /// </summary>
        public bool ShouldIncludeRuntimeDirectory(string runtimeDirectoryPath)
        {
            var output = runtimeDirectoryPath != Instances.RuntimeDirectoryPaths.NetCore;
            return output;
        }

        public bool ShouldIncludeCoreRuntimeDirectory(
            string projectFilePath)
        {
            var shouldInclude = Instances.ProjectFileOperator.InQueryProjectFileContext_Synchronous(
               projectFilePath,
               this.ShouldIncludeCoreRuntimeDirectory);

            return shouldInclude;
        }

        public bool ShouldIncludeCoreRuntimeDirectory(
            XElement projectElement)
        {
            // Is the project a web project?
            var sdk = Instances.ProjectXmlOperator.GetSdk(projectElement);

            var shouldInclude = sdk switch
            {
                // Publishing Blazor WebAssembly results in ALL required assemblies.
                F0020.IProjectSdkStrings.BlazorWebAssembly_Constant => false,
                // True for all others.
                _ => true,
            };

            return shouldInclude;
        }

        public bool ShouldIncludeWindowsRuntimeDirectory(
            XElement projectElement)
        {
            var targetFramework = Instances.ProjectXmlOperator.GetTargetFramework(projectElement);

            // Is the project a windows forms project?
            var isWindowsProject = Instances.StringOperator.Contains(
                targetFramework,
                "windows");

            return isWindowsProject;
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
                    var targetFramework = Instances.ProjectXmlOperator.GetTargetFramework(projectElement);

                    var isWebProject = Instances.ProjectSdkStringOperations.Is_WebSdk(sdk);
                    if(isWebProject)
                    {
                        if(targetFramework.StartsWith(
                            Instances.TargetFrameworkMonikerStrings.NET_6))
                        {
                            return Instances.RuntimeDirectoryPaths.AspNetCore_6;
                        }

                        // Else, use 5.0.
                        return Instances.RuntimeDirectoryPaths.AspNetCore_5;
                    }

                    // Is the project a windows forms project?
                    if(Instances.StringOperator.Contains(
                        targetFramework,
                        "windows"))
                    {
                        return Instances.RuntimeDirectoryPaths.WindowsFormsNetCore;
                    }

                    // Else, just the regular .NET.
                    return Instances.RuntimeDirectoryPaths.NetCore;
                });

            return reflectionRuntimeDirectory;
        }

        public string DetermineAspNetReflectionRuntimeDirectoryForProject(
            string projectFilePath)
        {
            var reflectionRuntimeDirectory = Instances.ProjectFileOperator.InQueryProjectFileContext_Synchronous(
                projectFilePath,
                this.DetermineAspNetReflectionRuntimeDirectoryForProject);

            return reflectionRuntimeDirectory;
        }

        /// <summary>
        /// Assumes that the project <em>should</em> have an ASP.NET Core runtime, just determines the version.
        /// You should test for whether that is the case before using this function.
        /// </summary>
        public string DetermineAspNetReflectionRuntimeDirectoryForProject(
            XElement projectElement)
        {
            var targetFramework = Instances.ProjectXmlOperator.GetTargetFramework(projectElement);

            if (targetFramework.StartsWith(
                Instances.TargetFrameworkMonikerStrings.NET_6))
            {
                return Instances.RuntimeDirectoryPaths.AspNetCore_6;
            }

            // Else, use 5.0.
            return Instances.RuntimeDirectoryPaths.AspNetCore_5;
        }

        /// <summary>
        /// Assumes that the project <em>should</em> have a Windows runtime, just determines the version.
        /// You should test for whether that is the case before using this function.
        /// </summary>
        public string DetermineWindowsReflectionRuntimeDirectoryForProject(
            XElement projectElement)
        {
            var targetFramework = Instances.ProjectXmlOperator.GetTargetFramework(projectElement);

            if (targetFramework.StartsWith(
                Instances.TargetFrameworkMonikerStrings.NET_6))
            {
                return Instances.RuntimeDirectoryPaths.WindowsFormsNetCore_6;
            }

            // Else, use 5.0.
            return Instances.RuntimeDirectoryPaths.WindowsFormsNetCore_5;
        }

        public string DetermineCoreRuntimeDirectoryForProject(
            string projectFilePath)
        {
            var coreReflectionRuntimeDirectory = Instances.ProjectFileOperator.InQueryProjectFileContext_Synchronous(
                projectFilePath,
                this.DetermineCoreRuntimeDirectoryForProject);

            return coreReflectionRuntimeDirectory;
        }

        public string DetermineCoreRuntimeDirectoryForProject(
            XElement projectElement)
        {
            var targetFramework = Instances.ProjectXmlOperator.GetTargetFramework(projectElement);

            if (targetFramework.StartsWith(
                Instances.TargetFrameworkMonikerStrings.NET_6))
            {
                return Instances.RuntimeDirectoryPaths.NetCore_6;
            }

            // Else, use 5.0.
            return Instances.RuntimeDirectoryPaths.NetCore_5;
        }

        public ProjectFileTuple[] CreateProjectFilesTuples(
            IList<string> projectFilePaths)
        {
            var projectFileTuples = projectFilePaths
                .Select(this.CreateProjectFilesTuple)
                .Now();

            return projectFileTuples;
        }

        public ProjectFileTuple CreateProjectFilesTuple(
            string projectFilePath)
        {
            var sdk = Instances.ProjectFileOperator.GetSdk(projectFilePath);

            var assemblyFilePath = sdk switch
            {
                F0020.IProjectSdkStrings.BlazorWebAssembly_Constant => Instances.FilePathOperator.Get_PublishWwwRootFrameworkDirectoryOutputAssemblyFilePath(projectFilePath),
                // Else
                _ => Instances.FilePathOperator.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath),
            };

            var documentationFilePath = sdk switch
            {
                F0020.IProjectSdkStrings.BlazorWebAssembly_Constant => Instances.FilePathOperator.Get_ReleaseDocumentationFilePath(projectFilePath),
                _ => Instances.ProjectPathsOperator.GetDocumentationFilePath_ForAssemblyFilePath(assemblyFilePath),
            };

            var projectFilesTuple = new ProjectFileTuple
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
            string documentationForAssemblyFilePath,
            ProjectDependenciesSet projectDependenciesSet)
        {
            var instanceDescriptorsWithoutProjectFile = this.ProcessAssemblyFile_AddRuntimeDirectoryPaths(
                projectFilePath,
                assemblyFilePath,
                documentationForAssemblyFilePath,
                projectDependenciesSet);

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
            string documentationForAssemblyFilePath,
            ProjectDependenciesSet projectDependenciesSet)
        {
            var runtimeDirectoryPaths = Instances.EnumerableOperator.Empty<string>();

            Instances.ProjectFileOperator.InReadonlyProjectFileContext_Synchronous(
                projectFilePath,
                projectElement =>
                {
                    var shouldIncludeCoreRuntimeDirectory = this.ShouldIncludeCoreRuntimeDirectory(projectElement);
                    if (shouldIncludeCoreRuntimeDirectory)
                    {
                        var coreRuntimeDirectory = this.DetermineCoreRuntimeDirectoryForProject(projectElement);

                        runtimeDirectoryPaths = runtimeDirectoryPaths
                            .Append(coreRuntimeDirectory)
                            ;
                    }

                    var shouldIncludeAspNetRuntimeDirectoryPath = this.ShouldIncludeAspNetRuntimeDirectory(
                        projectFilePath,
                        projectElement,
                        projectDependenciesSet);

                    if (shouldIncludeAspNetRuntimeDirectoryPath)
                    {
                        var aspNetCoreDirectory = this.DetermineAspNetReflectionRuntimeDirectoryForProject(projectElement);

                        runtimeDirectoryPaths = runtimeDirectoryPaths
                            .Append(aspNetCoreDirectory)
                            ;
                    }

                    var shouldIncludeWindowsRuntime = this.ShouldIncludeWindowsRuntimeDirectory(projectElement);
                    if(shouldIncludeWindowsRuntime)
                    {
                        var windowsDirectory = this.DetermineWindowsReflectionRuntimeDirectoryForProject(projectElement);

                        runtimeDirectoryPaths = runtimeDirectoryPaths
                            .Append(windowsDirectory)
                            ;
                    }
                });

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

        public async Task<InstanceDescriptor[]> ProcessBuiltProjects(
            IList<ProjectFileTuple> projectFileTuples,
            Func<IDictionary<string, string>, Task> processingProblemsConsumer,
            ILogger logger)
        {
            var (processingProblemTextsByProjectFilePath, instances) = await this.ProcessBuiltProjects(
                projectFileTuples,
                logger);

            await processingProblemsConsumer(processingProblemTextsByProjectFilePath);

            return instances;
        }

        public void ProcessingProblemsConsumer(
            IDictionary<string, string> processingProblemTextsByProjectFilePath,
            string processingProblemsTextFilePath,
            string processingProblemProjectsTextFilePath)
        {
            this.WriteProblemProjectsFile(
                processingProblemsTextFilePath,
                processingProblemTextsByProjectFilePath);

            Instances.FileOperator.WriteLines_Synchronous(
                processingProblemProjectsTextFilePath,
                processingProblemTextsByProjectFilePath.Keys
                    .OrderAlphabetically());
        }

        public async Task ProcessBuiltProjects(
            IList<ProjectFileTuple> projectFileTuples,
            string processingProblemsTextFilePath,
            string processingProblemProjectsTextFilePath,
            string instancesJsonFilePath,
            ILogger logger)
        {
            var instances = await this.ProcessBuiltProjects(
                projectFileTuples,
                processingProblemTextsByProjectFilePath =>
                {
                    this.ProcessingProblemsConsumer(
                        processingProblemTextsByProjectFilePath,
                        processingProblemsTextFilePath,
                        processingProblemProjectsTextFilePath);

                    return Task.CompletedTask;
                },
                logger);

            await Instances.JsonOperator.Serialize(
               instancesJsonFilePath,
               instances);
        }

        public async Task<(
            Dictionary<string, string> processingProblemTextsByProjectFilePath,
            InstanceDescriptor[] instances)>
            ProcessBuiltProjects(
            IList<ProjectFileTuple> projectFileTuples,
            ILogger logger)
        {
            var projectDependenciesSet = new ProjectDependenciesSet();

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

                // Now test for whether the assembly file exists.
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
                    // First get the recursive dependencies for a project.
                    await Instances.ProjectReferencesOperator.AddRecursiveProjectReferences_Exclusive_Idempotent(
                        projectDependenciesSet.RecursiveProjectDependenciesByProjectFilePath_Exclusive,
                        tuple.ProjectFilePath);

                    // Then compute which projects in the recursive set do not have information about whether they have an ASP.NET dependency.
                    static void ComputeAspNetCoreReferences(ProjectDependenciesSet projectDependenciesSet)
                    {
                        // Keep a list of projects for file data analysis.
                        var projectsToEvaluate = new HashSet<string>();

                        foreach (var project in projectDependenciesSet.RecursiveProjectDependenciesByProjectFilePath_Exclusive.Keys)
                        {
                            var alreadyEvaluated = projectDependenciesSet.HasAspNetDependencyByProjectFilePath.ContainsKey(project);
                            if (!alreadyEvaluated)
                            {
                                var hasAspNetDependency = false;

                                // First compute which projects have ASP.NET core references just by evaluating dependency references.
                                // (Do this first since it is quicker than going to the file system, and if a project has both a reference to a project with an ASP.NET Core reference, and an ASP.NET Core reference, it's quicker to mark it based on dependency reference data in memory than by checking the file system.)
                                var recursiveDependencies = projectDependenciesSet.RecursiveProjectDependenciesByProjectFilePath_Exclusive[project];
                                foreach (var dependencyProject in recursiveDependencies)
                                {
                                    var dependencyAlreadyEvaluated = projectDependenciesSet.HasAspNetDependencyByProjectFilePath.ContainsKey(dependencyProject);
                                    if (dependencyAlreadyEvaluated)
                                    {
                                        var dependencyHasAspNetDependency = projectDependenciesSet.HasAspNetDependencyByProjectFilePath[dependencyProject];
                                        if (dependencyHasAspNetDependency)
                                        {
                                            // If the project has a dependency that is known to have an ASP.NET Core dependency, then the project is known to have an ASP.NET Core dependency.
                                            hasAspNetDependency = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        // Add the dependency to the list of those needing evaluation.
                                        projectsToEvaluate.Add(dependencyProject);
                                    }
                                }

                                // Only if we know the project has an ASP.NET dependency based on dependency analysis do we add it, and are done.
                                if (hasAspNetDependency)
                                {
                                    projectDependenciesSet.HasAspNetDependencyByProjectFilePath.Add(
                                        project,
                                        hasAspNetDependency);

                                    continue;
                                }

                                // Now we have to query the file.
                                var hasAspNetCoreAppFrameworkReference = Instances.ProjectFileOperator.InQueryProjectFileContext_Synchronous(
                                    project,
                                    projectElement =>
                                    {
                                        var hasAspNetCoreAppFrameworkReference = Instances.ProjectXmlOperator.HasFrameworkReference(
                                            projectElement,
                                            Instances.FrameworkNames.Microsoft_AspNetCore_App);

                                        return hasAspNetCoreAppFrameworkReference;
                                    });

                                // Because we have previously tested for having a recursive ASP.NET reference, we now know definitively whether the project has a ASP.NET Core reference.
                                projectDependenciesSet.HasAspNetDependencyByProjectFilePath.Add(
                                    project,
                                    hasAspNetCoreAppFrameworkReference);
                            }
                        }
                    }

                    ComputeAspNetCoreReferences(projectDependenciesSet);

                    var currentInstances = Operations.Instance.ProcessAssemblyFile(
                        tuple.ProjectFilePath,
                        tuple.AssemblyFilePath,
                        tuple.DocumentationFilePath,
                        projectDependenciesSet);

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

        public async Task<Dictionary<string, string>> BuildProjectFilePaths(
            bool rebuildFailedBuildsToCollectErrors,
            IList<string> projectFilePaths,
            string buildProblemsFilePath,
            string buildProblemProjectsFilePath,
            HashSet<string> projectFilePathsToSkip,
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
                    projectFilePathsToSkip,
                    logger);

                if (shouldBuildProject)
                {
                    // Clear the publish directory and publish (build), and not any problems.
                    await this.BuildProject(
                        projectFilePath,
                        logger,
                        buildProblemTextsByProjectFilePath);
                }
                else
                {
                    // Is project on the skip list?
                    if(projectFilePathsToSkip.Contains(projectFilePath))
                    {
                        buildProblemTextsByProjectFilePath.Add(
                            projectFilePath,
                            "Project is on skip list.");

                        continue;
                    }

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
            await Instances.FileOperator.WriteLines(
                buildProblemProjectsFilePath,
                buildProblemTextsByProjectFilePath.Keys
                    .OrderAlphabetically());

            return buildProblemTextsByProjectFilePath;
        }

        /// <summary>
        /// Builds the project with the win-x64 runtime argument to get a self-contained executable.
        /// (To allow full assembly reflection.)
        /// </summary>
        public async Task BuildProject(
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
            await Instances.PublishOperator.Publish(
                projectFilePath,
                publishDirectoryPath);

            logger.LogInformation("Built project.");
        }

        public async Task BuildProject(
            string projectFilePath,
            ILogger logger,
            Dictionary<string, string> buildProblemTextsByProjectFilePath)
        {
            try
            {
                await this.BuildProject(
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
            HashSet<string> projectFilePathsToSkip,
            ILogger logger)
        {
            logger.LogInformation("Determining whether the project should be built:\n\t{projectFilePath}", projectFilePath);

            if(projectFilePathsToSkip.Contains(projectFilePath))
            {
                logger.LogInformation("Project is on skip list.");

                return false;
            }

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
        /// Outputs the projects to a text file.
        /// </summary>
        public void GetAllProjectFilePaths(
            string outputTextFilePath,
            ILogger logger)
        {
            var projectFilePaths = this.GetAllProjectFilePaths(
                logger);

            Instances.FileOperator.WriteLines_Synchronous(
                outputTextFilePath,
                projectFilePaths);
        }

        /// <summary>
        /// Searches the file-system in each of the repositories directory paths for projects.
        /// </summary>
        public string[] GetAllProjectFilePaths(
            ILogger logger)
        {
            // Output project paths to current run date's directory.
            var repositoriesDirectoryPaths = Instances.RepositoriesDirectoryPaths.AllOfMine;

            var projectFilePaths = Instances.FileSystemOperator.GetAllProjectFilePaths_FromRepositoriesDirectoryPaths(
                repositoriesDirectoryPaths,
                logger)
                .OrderAlphabetically()
                .Now();

            return projectFilePaths;
        }

        public void WriteProblemProjectsFile(
            string problemProjectsFilePath,
            IDictionary<string, string> problemProjects)
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
