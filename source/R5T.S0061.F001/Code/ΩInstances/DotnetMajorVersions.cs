using System;


namespace R5T.S0061.F001
{
    public class DotnetMajorVersions : IDotnetMajorVersions
    {
        #region Infrastructure

        public static IDotnetMajorVersions Instance { get; } = new DotnetMajorVersions();


        private DotnetMajorVersions()
        {
        }

        #endregion
    }
}
