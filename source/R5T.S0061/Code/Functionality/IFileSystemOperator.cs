using System;

using R5T.F0000;
using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IFileSystemOperator : IFunctionalityMarker,
        F0000.IFileSystemOperator,
        F0082.IFileSystemOperator
    {
        public bool Has_OutputAssembly(
            string projectFilePath)
        {
            var assemblyFilePath = Instances.FilePathOperator.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

            var outputAssemblyExists = Instances.FileSystemOperator.FileExists(assemblyFilePath);
            return outputAssemblyExists;
        }

        public bool Has_BuildResultFile(
            string projectFilePath)
        {
            var buildJsonFilePath = Instances.FilePathOperator.Get_BuildJsonFilePath(projectFilePath);

            var buildJsonFileExists = Instances.FileSystemOperator.FileExists(buildJsonFilePath);
            return buildJsonFileExists;
        }

        public WasFound<BuildResult> Has_BuildResult(
            string projectFilePath)
        {
            var buildJsonFilePath = Instances.FilePathOperator.Get_BuildJsonFilePath(projectFilePath);

            var buildJsonFileExists = Instances.FileSystemOperator.FileExists(buildJsonFilePath);

            var buildResultOrDefault = buildJsonFileExists
                ? Instances.JsonOperator.Deserialize_Synchronous<BuildResult>(buildJsonFilePath)
                : default
                ;

            var hasBuildResult = WasFound.From(buildResultOrDefault);
            return hasBuildResult;
        }
    }
}
