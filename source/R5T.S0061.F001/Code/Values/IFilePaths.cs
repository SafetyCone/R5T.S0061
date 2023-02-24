using System;

using R5T.T0131;


namespace R5T.S0061.F001
{
    [ValuesMarker]
    public partial interface IFilePaths : IValuesMarker
    {
        /// <inheritdoc cref="Documentation.DoNotBuildProjectsListTextFilePath"/>
        public string DoNotBuildProjectsListTextFilePath => @"C:\Users\David\Dropbox\Organizations\Rivet\Shared\Data\Do not build Projects.txt";

        public string InstancesJsonFilePath => @"C:\Users\David\Dropbox\Organizations\Rivet\Shared\Data\Instances\Instances.json";
    }
}
