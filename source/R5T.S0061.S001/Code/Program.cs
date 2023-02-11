using System;
using System.Threading.Tasks;


namespace R5T.S0061.S001
{
    class Program
    {
        static async Task Main()
        {
            //Scripts.Instance.GetAllProjectFilePaths();
            await Scripts.Instance.BuildAllProjectFilePaths();
            //Scripts.Instance.ProcessProjects();

            //Scripts.Instance.SummarizeProcessingProblems();

            //Scripts.Instance.SearchInstances_NameContainsText();
            //Scripts.Instance.SearchFunctionality_NameContainsText_Categorize();
        }
    }
}