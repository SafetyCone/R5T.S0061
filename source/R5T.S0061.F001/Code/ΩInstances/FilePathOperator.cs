using System;


namespace R5T.S0061.F001
{
    public class FilePathOperator : IFilePathOperator
    {
        #region Infrastructure

        public static IFilePathOperator Instance { get; } = new FilePathOperator();


        private FilePathOperator()
        {
        }

        #endregion
    }
}
