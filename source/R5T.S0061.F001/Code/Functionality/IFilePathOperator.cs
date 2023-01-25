using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IFilePathOperator : IFunctionalityMarker
    {
        public string Get_BuildJsonFilePath(string projectFilePath)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var buildJsonFilePath = this.Get_BuildJsonFilePath_FromPublishDirectory(publishDirectoryPath);
            return buildJsonFilePath;
        }

        public string Get_BuildJsonFilePath_FromPublishDirectory(string publishDirectoryPath)
        {
            var buildJsonFilePath = Instances.PathOperator.GetFilePath(
                publishDirectoryPath,
                FileNames.Instance.BuildJsonFileName);

            return buildJsonFilePath;
        }

        public string Get_PublishDirectoryOutputAssemblyFilePath(
            string projectFilePath)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var projectName = Instances.ProjectPathsOperator.GetProjectName(projectFilePath);

            var outputAssemblyFileName = F0000.FileNameOperator.Instance.GetFileName(
                projectName,
                Instances.FileExtensions.Dll);

            var assemblyFilePath = Instances.PathOperator.GetFilePath(
                publishDirectoryPath,
                outputAssemblyFileName);

            return assemblyFilePath;
        }
    }
}
