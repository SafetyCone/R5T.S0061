using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IDirectoryPathOperator : IFunctionalityMarker
    {
        public string GetPublishDirectoryPath_ForProjectFilePath(string projectFilePath)
        {
            var projectDirectoryPath = F0052.ProjectPathsOperator.Instance.GetProjectDirectoryPath(projectFilePath);

            var publishDirectoryPath = F0002.PathOperator.Instance.GetDirectoryPath(
                projectDirectoryPath,
                Instances.DirectoryNames.bin,
                Instances.DirectoryNames.Publish);

            return publishDirectoryPath;
        }
    }
}
