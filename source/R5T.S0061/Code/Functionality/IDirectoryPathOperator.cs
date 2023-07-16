using System;

using R5T.T0132;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IDirectoryPathOperator : IFunctionalityMarker,
        F001.IDirectoryPathOperator
    {
        public string Get_ApplicationOutputDirectoryPath()
        {
            var datedOutputDirectoryPath = this.Get_ApplicationSpecificOutputDirectoryPath(
                Instances.DirectoryPaths.CloudSharedOutputDirectoryPath.Value,
                Instances.Values.ApplicationName);

            return datedOutputDirectoryPath;
        }
    }
}
