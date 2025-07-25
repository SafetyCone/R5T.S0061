using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using System.Extensions;

using R5T.F0114;
using R5T.T0132;
using R5T.T0159;
using R5T.T0159.Extensions;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker,
        F0111.IOperations,
        F0112.IOperations,
        F0112.Internal.IOperations
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
            ProjectDependenciesSet projectDependenciesSet,
            DotnetRuntimeDirectoryPaths dotnetRuntimeDirectoryPaths)
        {
            var instanceDescriptorsWithoutProjectFile = this.ProcessAssemblyFile_AddRuntimeDirectoryPaths(
                projectFilePath,
                assemblyFilePath,
                documentationForAssemblyFilePath,
                projectDependenciesSet,
                dotnetRuntimeDirectoryPaths);

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
            ProjectDependenciesSet projectDependenciesSet,
            DotnetRuntimeDirectoryPaths dotnetRuntimeDirectoryPaths)
        {
            var runtimeDirectoryPaths = Instances.EnumerableOperator.Empty<string>();

            Instances.ProjectFileOperator.InReadonlyProjectFileContext_Synchronous(
                projectFilePath,
                projectElement =>
                {
                    var shouldIncludeCoreRuntimeDirectory = this.ShouldIncludeCoreRuntimeDirectory(projectElement);
                    if (shouldIncludeCoreRuntimeDirectory)
                    {
                        runtimeDirectoryPaths = runtimeDirectoryPaths
                            .Append(dotnetRuntimeDirectoryPaths.NetCoreApp)
                            ;
                    }

                    var shouldIncludeAspNetRuntimeDirectoryPath = this.ShouldIncludeAspNetRuntimeDirectory(
                        projectFilePath,
                        projectElement,
                        projectDependenciesSet);

                    if (shouldIncludeAspNetRuntimeDirectoryPath)
                    {
                        runtimeDirectoryPaths = runtimeDirectoryPaths
                            .Append(dotnetRuntimeDirectoryPaths.AspNetCoreApp)
                            ;
                    }

                    var shouldIncludeWindowsRuntime = this.ShouldIncludeWindowsRuntimeDirectory(projectElement);
                    if(shouldIncludeWindowsRuntime)
                    {
                        runtimeDirectoryPaths = runtimeDirectoryPaths
                            .Append(dotnetRuntimeDirectoryPaths.WindowsDesktopApp)
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
            ITextOutput textOutput)
        {
            var (processingProblemTextsByProjectFilePath, instances) = await this.ProcessBuiltProjects(
                projectFileTuples,
                textOutput);

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

            Instances.FileOperator.Write_Lines_Synchronous(
                processingProblemProjectsTextFilePath,
                processingProblemTextsByProjectFilePath.Keys
                    .OrderAlphabetically());
        }

        public async Task ProcessBuiltProjects(
            IList<ProjectFileTuple> projectFileTuples,
            string processingProblemsTextFilePath,
            string processingProblemProjectsTextFilePath,
            string instancesJsonFilePath,
            ITextOutput textOutput)
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
                textOutput);

            await Instances.JsonOperator.Serialize(
               instancesJsonFilePath,
               instances);
        }

        public async Task<(
            Dictionary<string, string> processingProblemTextsByProjectFilePath,
            InstanceDescriptor[] instances)>
            ProcessBuiltProjects(
            IList<ProjectFileTuple> projectFileTuples,
            ITextOutput textOutput)
        {
            var projectDependenciesSet = new ProjectDependenciesSet();

            var instances = new List<InstanceDescriptor>();

            var processingProblemTextsByProjectFilePath = new Dictionary<string, string>();

            var projectCounter = 1; // Start at 1.
            var projectCount = projectFileTuples.Count;

            // Get the current latest runtime directory paths.
            var dotnetRuntimeDirectoryPaths = Instances.RuntimeDirectoryPathOperator.GetDotnetRuntimeDirectoryPaths();

            // Now process each project.
            foreach (var tuple in projectFileTuples)
            {
                textOutput.WriteInformation("Processing project ({projectCounter} / {projectCount}):\n\t{projectFile}",
                    projectCounter++,
                    projectCount,
                    tuple.ProjectFilePath);

                // Now test for whether the assembly file exists.
                var assemblyFileExists = Instances.FileSystemOperator.Exists_File(
                        tuple.AssemblyFilePath);

                if (!assemblyFileExists)
                {
                    processingProblemTextsByProjectFilePath.Add(
                        tuple.ProjectFilePath,
                        "No assembly file.");

                    textOutput.WriteInformation("No assembly file to process, build failed.");

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
                        projectDependenciesSet,
                        dotnetRuntimeDirectoryPaths);

                    instances.AddRange(currentInstances);

                    textOutput.WriteInformation("Processed project.");
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

        /// <summary>
        /// <inheritdoc cref="Documentation.ProjectsListTextFilePath" path="/summary"/>
        /// Outputs the projects to a text file.
        /// DOES NOT FILTER any projects.
        /// </summary>
        public void GetAllProjectFilePaths_WithoutRemovingProjects(
            string outputTextFilePath,
            ILogger logger)
        {
            var projectFilePaths = this.Find_AllProjectFilePaths(
                logger.ToTextOutput());

            Instances.FileOperator.Write_Lines_Synchronous(
                outputTextFilePath,
                projectFilePaths);
        }
    }
}
