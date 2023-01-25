using System;


namespace R5T.S0061.F001
{
    public class RuntimeDirectoryPaths : IRuntimeDirectoryPaths
    {
        #region Infrastructure

        public static IRuntimeDirectoryPaths Instance { get; } = new RuntimeDirectoryPaths();


        private RuntimeDirectoryPaths()
        {
        }

        #endregion
    }
}
