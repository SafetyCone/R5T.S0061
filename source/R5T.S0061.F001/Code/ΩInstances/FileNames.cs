using System;


namespace R5T.S0061.F001
{
    public class FileNames : IFileNames
    {
        #region Infrastructure

        public static IFileNames Instance { get; } = new FileNames();


        private FileNames()
        {
        }

        #endregion
    }
}
