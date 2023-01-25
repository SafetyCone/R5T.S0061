using System;


namespace R5T.S0061.F001
{
    public static class Instances
    {
        public static F0000.IAssemblyOperator AssemblyOperator => F0000.AssemblyOperator.Instance;
        public static Z0012.IDirectoryNameOperator DirectoryNameOperator => Z0012.DirectoryNameOperator.Instance;
        public static Z0012.IDirectoryNames DirectoryNames => Z0012.DirectoryNames.Instance;
        public static IDirectoryPathOperator DirectoryPathOperator => F001.DirectoryPathOperator.Instance;
        public static F0099.IDocumentationOperations DocumentationOperations => F0099.DocumentationOperations.Instance;
        public static F0027.IDotnetPublishOperator DotnetPublishOperator => F0027.DotnetPublishOperator.Instance;
        public static F0000.IEnumerableOperator EnumerableOperator => F0000.EnumerableOperator.Instance;
        public static Z0010.IFileExtensions FileExtensions => Z0010.FileExtensions.Instance;
        public static IFileNames FileNames => F001.FileNames.Instance;
        public static F0000.IFileOperator FileOperator => F0000.FileOperator.Instance;
        public static IFilePathOperator FilePathOperator => F001.FilePathOperator.Instance;
        public static IFileSystemOperator FileSystemOperator => F001.FileSystemOperator.Instance;
        public static F0017.F002.IIdentityNameProvider IdentityNameProvider => F0017.F002.IdentityNameProvider.Instance;
        public static Z001.IInstanceVariety InstanceVariety => Z001.InstanceVariety.Instance;
        public static F0032.IJsonOperator JsonOperator => F0032.JsonOperator.Instance;
        public static Z0006.INamespacedTypeNames NamespacedTypeNames => Z0006.NamespacedTypeNames.Instance;
        public static F0000.INowOperator NowOperator => F0000.NowOperator.Instance;
        public static F0017.F002.IParameterNamedIdentityNameProvider ParameterNamedIdentityNameProvider => F0017.F002.ParameterNamedIdentityNameProvider.Instance;
        public static F0002.IPathOperator PathOperator => F0002.PathOperator.Instance;
        public static F0020.IProjectFileOperator ProjectFileOperator => F0020.ProjectFileOperator.Instance;
        public static F0020.IProjectSdkStringOperations ProjectSdkStringOperations => F0020.ProjectSdkStringOperations.Instance;
        public static F0020.IProjectSdkStrings ProjectSdkStrings => F0020.ProjectSdkStrings.Instance;
        public static IProjectPathsOperator ProjectPathsOperator => F001.ProjectPathsOperator.Instance;
        public static F0020.IProjectXmlOperations ProjectXmlOperations => F0020.ProjectXmlOperations.Instance;
        public static F0020.IProjectXmlOperator ProjectXmlOperator => F0020.ProjectXmlOperator.Instance;
        public static IReflectionOperations ReflectionOperations => F001.ReflectionOperations.Instance;
        public static F0018.IReflectionOperator ReflectionOperator => F0018.ReflectionOperator.Instance;
        public static Z0022.IRepositoriesDirectoryPaths RepositoriesDirectoryPaths => Z0022.RepositoriesDirectoryPaths.Instance;
        public static IRuntimeDirectoryPaths RuntimeDirectoryPaths => F001.RuntimeDirectoryPaths.Instance;
        public static F0000.IStringOperator StringOperator => F0000.StringOperator.Instance;
        public static F0000.ITextOperator TextOperator => F0000.TextOperator.Instance;
    }
}