using System;


namespace R5T.S0061
{
    public class FileSystemOperator : IFileSystemOperator
    {
        #region Infrastructure

        public static IFileSystemOperator Instance { get; } = new FileSystemOperator();


        private FileSystemOperator()
        {
        }

        #endregion
    }
}
