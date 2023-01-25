using System;

using R5T.T0132;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IFilePathOperator : IFunctionalityMarker,
        F001.IFilePathOperator
    {
        public string Get_NewAndOldSummaryTextFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.NewAndOldSummaryTextFilePath);

            return instancesJsonFilePath;
        }

        public string Get_DateComparisonSummaryTextFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.DateComparisonSummaryTextFilePath);

            return instancesJsonFilePath;
        }

        public string Get_NewInstancesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.NewInstancesJsonFileName);

            return instancesJsonFilePath;
        }

        public string Get_OldInstancesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.OldInstancesJsonFileName);

            return instancesJsonFilePath;
        }

        public string Get_ProcessingSummaryTextFilePath(
            string datedOutputDirectoryPath)
        {
            var summaryFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.ProcessingSummaryTextFileName);

            return summaryFilePath;
        }

        public string Get_InstancesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var instancesJsonFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.InstancesJsonFileName);

            return instancesJsonFilePath;
        }

        public string Get_ProcessingProblemProjectsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemProjectsFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.ProcessingProblemProjectsTextFileName);

            return buildProblemProjectsFilePath;
        }

        public string Get_ProcessingProblemsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemsFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.ProcessingProblemsTextFileName);

            return buildProblemsFilePath;
        }

        public string Get_ProjectFileTuplesJsonFilePath(
            string datedOutputDirectoryPath)
        {
            var projectFileTuplesJsonFilePath = Instances.PathOperator.GetFilePath(
                datedOutputDirectoryPath,
                Instances.FileNames.ProjectFileTuplesJsonFileName);

            return projectFileTuplesJsonFilePath;
        }

        public string Get_BuildProblemProjectsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemProjectsFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.BuildProblemProjectsTextFileName);

            return buildProblemProjectsFilePath;
        }

        public string Get_BuildProblemsTextFilePath(
            string datedOutputDirectoryPath)
        {
            var buildProblemsFilePath = F0002.PathOperator.Instance.GetFilePath(
                datedOutputDirectoryPath,
                FileNames.Instance.BuildProblemsTextFileName);

            return buildProblemsFilePath;
        }

        public string Get_ProjectsListTextFilePath(
            string datedOutputDirectoryPath)
        {
            var projectsListTextFilePath = Instances.PathOperator.GetFilePath(
                datedOutputDirectoryPath,
                Instances.FileNames.ProjectsListTextFileName);

            return projectsListTextFilePath;
        }
    }
}
