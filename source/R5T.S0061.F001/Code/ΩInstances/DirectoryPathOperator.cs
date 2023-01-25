using System;


namespace R5T.S0061.F001
{
    public class DirectoryPathOperator : IDirectoryPathOperator
    {
        #region Infrastructure

        public static IDirectoryPathOperator Instance { get; } = new DirectoryPathOperator();


        private DirectoryPathOperator()
        {
        }

        #endregion
    }
}
