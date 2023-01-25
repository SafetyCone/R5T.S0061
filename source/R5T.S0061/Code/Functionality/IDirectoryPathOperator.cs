using System;

using R5T.T0132;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IDirectoryPathOperator : IFunctionalityMarker,
        F0098.IDirectoryPathOperator,
        F001.IDirectoryPathOperator
    {
        public string Get_ApplicationOutputDirectoryPath()
        {
            var datedOutputDirectoryPath = this.Get_ApplicationSpecificOutputDirectoryPath(
                Instances.DirectoryPaths.CloudSharedOutputDirectoryPath,
                Instances.Values.ApplicationName);

            return datedOutputDirectoryPath;
        }

        public string Get_DatedOutputDirectoryPath(DateTime date)
        {
            var datedOutputDirectoryPath = this.Get_DatedApplicationSpecificOutputDirectoryPath(
                Instances.DirectoryPaths.CloudSharedOutputDirectoryPath,
                Instances.Values.ApplicationName,
                date);

            return datedOutputDirectoryPath;
        }
    }
}
