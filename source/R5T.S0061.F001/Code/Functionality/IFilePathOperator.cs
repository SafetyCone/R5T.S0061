using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IFilePathOperator : IFunctionalityMarker,
        F0112.IFilePathOperator
    {
        public string Get_PriorToDateFilePath(
            string filePath,
            DateTime date)
        {
            var priorToDateFilePath = Instances.PathOperator.Append_ToFileNameStem(
                filePath,
                fileName => Instances.FileNameOperator.GetPriorToDateFileName(
                    fileName,
                    date));

            return priorToDateFilePath;
        }

        public string Get_ReleaseDocumentationFilePath(
            string projectFilePath)
        {
            var projectDirectoryPath = F0052.ProjectPathsOperator.Instance.GetProjectDirectoryPath(projectFilePath);

            var releaseDirectoryPath = F0002.PathOperator.Instance.Get_DirectoryPath(
                projectDirectoryPath,
                Instances.DirectoryNames.bin,
                Instances.DirectoryNames.Release);

            var documentationFileName = Instances.ProjectPathsOperator.GetDocumentationFileName_FromProjectFilePath(projectFilePath);

            var documentationFilePath = Instances.PathOperator.Get_FilePath(
                releaseDirectoryPath,
                documentationFileName);

            return documentationFilePath;
        }
    }
}
