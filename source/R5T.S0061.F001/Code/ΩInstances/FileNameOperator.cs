using System;


namespace R5T.S0061.F001
{
    public class FileNameOperator : IFileNameOperator
    {
        #region Infrastructure

        public static IFileNameOperator Instance { get; } = new FileNameOperator();


        private FileNameOperator()
        {
        }

        #endregion
    }
}
