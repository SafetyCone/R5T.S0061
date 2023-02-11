using System;
using System.Threading.Tasks;


namespace R5T.S0061
{
    class Program
    {
        static async Task Main()
        {
            await Operations.Instance.Run();
        }
    }
}