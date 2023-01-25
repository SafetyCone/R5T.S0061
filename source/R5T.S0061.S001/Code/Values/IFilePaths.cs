using System;

using R5T.T0131;


namespace R5T.S0061.S001
{
    [ValuesMarker]
    public partial interface IFilePaths : IValuesMarker
    {
        public string BuildProblemProjectsTextFilePath => @"C:\Temp\Build Problem Projects.txt";
        public string BuildProblemsTextFilePath => @"C:\Temp\Build Problems.txt";
        public string ProjectsListTextFilePath => @"C:\Temp\Projects List.txt";

        public string ProcessingProblemsTextFilePath => @"C:\Temp\Processing Problems.txt";
        public string ProcessingProblemProjectsTextFilePath => @"C:\Temp\Processing Problem Projects.txt";
        public string InstancesJsonFilePath => @"C:\Temp\Instances.json";
    }
}
