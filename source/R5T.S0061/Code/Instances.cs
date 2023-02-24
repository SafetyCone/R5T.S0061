using System;


namespace R5T.S0061
{
    public static class Instances
    {
        public static F0000.IDateOperator DateOperator => F0000.DateOperator.Instance;
        public static Z0012.IDirectoryNameOperator DirectoryNameOperator => Z0012.DirectoryNameOperator.Instance;
        public static IDirectoryPathOperator DirectoryPathOperator => S0061.DirectoryPathOperator.Instance;
        public static Z0012.IDirectoryNames DirectoryNames => Z0012.DirectoryNames.Instance;
        public static Z0026.IDirectoryPaths DirectoryPaths => Z0026.DirectoryPaths.Instance;
        public static F0099.IDocumentationOperations DocumentationOperations => F0099.DocumentationOperations.Instance;
        public static F0027.IDotnetPublishOperator DotnetPublishOperator => F0027.DotnetPublishOperator.Instance;
        public static D8S.Z0003.IEmailAddresses EmailAddresses => D8S.Z0003.EmailAddresses.Instance;
        public static F0097.IEmailSender EmailSender => F0097.EmailSender.Instance;
        public static F0000.IEnumerableOperator EnumerableOperator => F0000.EnumerableOperator.Instance;
        public static Z0010.IFileExtensions FileExtensions => Z0010.FileExtensions.Instance;
        public static IFileNames FileNames => S0061.FileNames.Instance;
        public static IFileNameOperator FileNameOperator => S0061.FileNameOperator.Instance;
        public static F0000.IFileOperator FileOperator => F0000.FileOperator.Instance;
        public static IFilePathOperator FilePathOperator => S0061.FilePathOperator.Instance;
        public static IFilePaths FilePaths => S0061.FilePaths.Instance;
        public static IFileSystemOperator FileSystemOperator => S0061.FileSystemOperator.Instance;
        public static T0158.F000.IHumanOutputTextFilePathOperator HumanOutputTextFilePathOperator => T0158.F000.HumanOutputTextFilePathOperator.Instance;
        public static F0017.F002.IIdentityNameProvider IdentityNameProvider => F0017.F002.IdentityNameProvider.Instance;
        public static Z001.IInstanceVariety InstanceVariety => Z001.InstanceVariety.Instance;
        public static IInstanceVarietyOperator InstanceVarietyOperator => S0061.InstanceVarietyOperator.Instance;
        public static F0032.IJsonOperator JsonOperator => F0032.JsonOperator.Instance;
        public static T0148.ILogFilePathOperator LogFilePathOperator => T0148.LogFilePathOperator.Instance;
        public static F0035.ILoggingOperator LoggingOperator => F0035.LoggingOperator.Instance;
        public static Z0006.INamespacedTypeNames NamespacedTypeNames => Z0006.NamespacedTypeNames.Instance;
        public static F0000.INowOperator NowOperator => F0000.NowOperator.Instance;
        public static F001.IOutputOperations OutputOperations => F001.OutputOperations.Instance;
        public static F0002.IPathOperator PathOperator => F0002.PathOperator.Instance;
        public static F0017.F002.IParameterNamedIdentityNameProvider ParameterNamedIdentityNameProvider => F0017.F002.ParameterNamedIdentityNameProvider.Instance;
        public static F0052.IProjectPathsOperator ProjectPathsOperator => F0052.ProjectPathsOperator.Instance;
        public static F0018.IReflectionOperator ReflectionOperator => F0018.ReflectionOperator.Instance;
        public static F0018.IReflectionOperations ReflectionOperations => F0018.ReflectionOperations.Instance;
        public static Z0022.IRepositoriesDirectoryPaths RepositoriesDirectoryPaths => Z0022.RepositoriesDirectoryPaths.Instance;
        public static F0000.IStringOperator StringOperator => F0000.StringOperator.Instance;
        public static Z0000.IStrings Strings => Z0000.Strings.Instance;
        public static T0159.F000.ITextOutputOperator TextOutputOperator => T0159.F000.TextOutputOperator.Instance;
        public static IValues Values => S0061.Values.Instance;
    }
}