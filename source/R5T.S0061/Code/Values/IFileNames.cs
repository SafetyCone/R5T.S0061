using System;

using R5T.T0131;


namespace R5T.S0061
{
    [ValuesMarker]
    public partial interface IFileNames : IValuesMarker,
        F001.IFileNames
    {
        public string BuildProblemsTextFileName => "Build Problems.txt";
        public string BuildProblemProjectsTextFileName => "Build Problem Projects.txt";
        public string DateComparisonSummaryTextFilePath => "Summary-Date Comparison.txt";
        public string InstancesJsonFileName => "Instances.json";
        public string NewAndOldSummaryTextFilePath => "Summary-New and Old.txt";
        public string NewInstancesJsonFileName => "Instances-New.json";
        public string OldInstancesJsonFileName => "Instances-Old.json";
        public string ProblemProcessingProjectsTextFileName => "Problem Processing Projects.txt";
        public string ProcessingProblemsTextFileName => "Processing Problems.txt";
        public string ProcessingProblemProjectsTextFileName => "Processing Problem Projects.txt";
        public string ProcessingSummaryTextFileName => "Summary-Processing.txt";
        public string ProcessedProjectsTextFileName => "Processed Projects.txt";
        public string ProjectFileTuplesJsonFileName => "Project File Tuples.json";

        /// <inheritdoc cref="F001.Documentation.ProjectsListTextFilePath"/>
        public string ProjectsListTextFileName => "Projects.txt";

        /// <inheritdoc cref="F001.Documentation.ProjectsList_AllTextFilePath"/>
        public string ProjectsList_AllTextFileName => "Projects-All.txt";

        public string SummaryTextFileName => "Summary.txt";
    }
}
