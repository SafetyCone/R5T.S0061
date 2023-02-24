using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IFilePathOperator : IFunctionalityMarker
    {
        public string Get_PriorToDateFilePath(
            string filePath,
            DateTime date)
        {
            var priorToDateFilePath = Instances.PathOperator.AppendToFileNameStem(
                filePath,
                fileName => Instances.FileNameOperator.GetPriorToDateFileName(
                    fileName,
                    date));

            return priorToDateFilePath;
        }

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

        public string Get_PublishWwwRootFrameworkDirectoryOutputAssemblyFilePath(
            string projectFilePath)
        {
            var publishDirectoryPath = Instances.DirectoryPathOperator.GetPublishDirectoryPath_ForProjectFilePath(projectFilePath);

            var wwwRootDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                publishDirectoryPath,
                Instances.DirectoryNames.WwwRoot);

            var frameworkDirectoryPath = Instances.PathOperator.GetDirectoryPath(
                wwwRootDirectoryPath,
                Instances.DirectoryNames.Framework);

            var projectName = Instances.ProjectPathsOperator.GetProjectName(projectFilePath);

            var outputAssemblyFileName = F0000.FileNameOperator.Instance.GetFileName(
                projectName,
                Instances.FileExtensions.Dll);

            var assemblyFilePath = Instances.PathOperator.GetFilePath(
                frameworkDirectoryPath,
                outputAssemblyFileName);

            return assemblyFilePath;
        }

        public string Get_ReleaseDocumentationFilePath(
            string projectFilePath)
        {
            var projectDirectoryPath = F0052.ProjectPathsOperator.Instance.GetProjectDirectoryPath(projectFilePath);

            var releaseDirectoryPath = F0002.PathOperator.Instance.GetDirectoryPath(
                projectDirectoryPath,
                Instances.DirectoryNames.bin,
                Instances.DirectoryNames.Release);

            var documentationFileName = Instances.ProjectPathsOperator.GetDocumentationFileName_FromProjectFilePath(projectFilePath);

            var documentationFilePath = Instances.PathOperator.GetFilePath(
                releaseDirectoryPath,
                documentationFileName);

            return documentationFilePath;
        }
    }
}
