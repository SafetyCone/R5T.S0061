using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IFileSystemOperator : IFunctionalityMarker,
        F0000.IFileSystemOperator,
        F0082.IFileSystemOperator
    {
        public bool Has_BuildResultFile(
            string projectFilePath)
        {
            var buildJsonFilePath = Instances.FilePathOperator.Get_BuildJsonFilePath(projectFilePath);

            var buildJsonFileExists = Instances.FileSystemOperator.FileExists(buildJsonFilePath);
            return buildJsonFileExists;
        }

        public bool Has_OutputAssembly(
            string projectFilePath)
        {
            // Test for the default output assembly file path.
            var assemblyFilePath = Instances.FilePathOperator.Get_PublishDirectoryOutputAssemblyFilePath(projectFilePath);

            var outputAssemblyExists = Instances.FileSystemOperator.FileExists(assemblyFilePath);
            if(outputAssemblyExists)
            {
                return true;
            }

            // Otherwise, test for the Blazor WebAssembly output file path.
            var blazorWebAssemblyFilePath = Instances.FilePathOperator.Get_PublishWwwRootFrameworkDirectoryOutputAssemblyFilePath(projectFilePath);

            var blazorWebAssemblyFileExists = Instances.FileSystemOperator.FileExists(blazorWebAssemblyFilePath);
            return blazorWebAssemblyFileExists;
        }
    }
}
