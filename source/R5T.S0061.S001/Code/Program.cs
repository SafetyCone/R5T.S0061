using System;
using System.Threading.Tasks;


namespace R5T.S0061.S001
{
    class Program
    {
        static async Task Main()
        {
            //Scripts.Instance.GetAllProjectFilePaths();
            //await Scripts.Instance.BuildAllProjectFilePaths();
            //await Scripts.Instance.ProcessProjects();

            //Scripts.Instance.SummarizeProcessingProblems();

            //Scripts.Instance.GetAllNuGetProjectFilePaths();
            //Scripts.Instance.CleanDoNotBuildProjectsFile();
            //Scripts.Instance.DeleteBinAndObjDirectories();
            //await Scripts.Instance.ProcessProject();
            //Scripts.Instance.SearchInstances_NameContainsText();
            //Scripts.Instance.SearchFunctionality_NameContainsText_Categorize();
            //Scripts.Instance.ComputeProcessingProblemsThatAreNotBuildProblems();

            Scripts.Instance.SummarizeInstances();
        }
    }
}