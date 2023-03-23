using System;
using System.Threading.Tasks;


namespace R5T.S0061.S001
{
    class Program
    {
        static async Task Main()
        {
            /// Main scripts.
            //Scripts.Instance.Find_AllProjectFilePaths();
            //await Scripts.Instance.BuildProjectFilePaths();
            await Scripts.Instance.ProcessProjects();
            //Scripts.Instance.CompareInstances();
            //Scripts.Instance.UpdateInstancesFile();
            //Scripts.Instance.OutputInstancesToFilesAndOpen();

            /// Search scripts.
            //Scripts.Instance.SearchInstances_NameContainsText();
            //Scripts.Instance.SearchFunctionality_NameContainsText_Categorize();

            /// Miscellaneous scripts.
            //Scripts.Instance.OpenInstanceFiles();

            //Scripts.Instance.SummarizeProcessingProblems();

            //Scripts.Instance.GetAllNuGetProjectFilePaths();
            //Scripts.Instance.CleanDoNotBuildProjectsListTextFile();
            //Scripts.Instance.DeleteBinAndObjDirectories();
            //await Scripts.Instance.BuildProjectFilePath();
            //await Scripts.Instance.ProcessProject();
            //Scripts.Instance.ComputeProcessingProblemsThatAreNotBuildProblems();

            //Scripts.Instance.SummarizeInstances();

        }
    }
}