using System;


namespace R5T.S0061.F001
{
    public class OutputOperations : IOutputOperations
    {
        #region Infrastructure

        public static IOutputOperations Instance { get; } = new OutputOperations();


        private OutputOperations()
        {
        }

        #endregion
    }
}
