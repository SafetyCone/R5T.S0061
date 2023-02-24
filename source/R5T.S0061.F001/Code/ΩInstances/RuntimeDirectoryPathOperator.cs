using System;


namespace R5T.S0061.F001
{
    public class RuntimeDirectoryPathOperator : IRuntimeDirectoryPathOperator
    {
        #region Infrastructure

        public static IRuntimeDirectoryPathOperator Instance { get; } = new RuntimeDirectoryPathOperator();


        private RuntimeDirectoryPathOperator()
        {
        }

        #endregion
    }
}
