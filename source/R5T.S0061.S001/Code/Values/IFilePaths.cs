using System;

using R5T.T0131;


namespace R5T.S0061.S001
{
    [ValuesMarker]
    public partial interface IFilePaths : IValuesMarker,
        F001.IFilePaths,
        Z0015.IFilePaths
    {
        public string BuildProblemProjectsTextFilePath => @"C:\Temp\Build Problem Projects.txt";
        public string BuildProblemsTextFilePath => @"C:\Temp\Build Problems.txt";

        /// <inheritdoc cref="F001.Documentation.ProjectsList_AllTextFilePath"/>
        public string ProjectsList_AllTextFilePath => @"C:\Temp\Projects List-All.txt";

        /// <inheritdoc cref="F001.Documentation.ProjectsListTextFilePath"/>
        public string ProjectsListTextFilePath => @"C:\Temp\Projects List.txt";

        public string ProcessingProblemsTextFilePath => @"C:\Temp\Processing Problems.txt";
        public string ProcessingProblemProjectsTextFilePath => @"C:\Temp\Processing Problem Projects.txt";

        public string Instances_PerRunJsonFilePath => @"C:\Temp\Instances-Per Run.json";

        public string NewInstancesJsonFilePath => @"C:\Temp\Instances-New.json";
        public string RemovedInstancesJsonFilePath => @"C:\Temp\Instances-Removed.json";
    }
}
