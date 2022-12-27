using System;


namespace R5T.S0061.Deploy
{
    public static class Instances
    {
        public static R5T.F0096.IAwsAuthenticationOperator AwsAuthenticationOperator => R5T.F0096.AwsAuthenticationOperator.Instance;
        public static R5T.Z0024.IDirectoryPaths DirectoryPaths => R5T.Z0024.DirectoryPaths.Instance;
        public static R5T.Z0017.IFilePaths FilePaths => R5T.Z0017.FilePaths.Instance;
        public static R5T.F0035.ILoggingOperator LoggingOperator => R5T.F0035.LoggingOperator.Instance;
        public static R5T.F0026.IProjectPathConventions ProjectPathConventions => R5T.F0026.ProjectPathConventions.Instance;
        public static R5T.L0028.IPublicationOperator PublicationOperator => R5T.L0028.PublicationOperator.Instance;
        public static R5T.L0028.IPublicationPathsOperator PublicationPathsOperator => R5T.L0028.PublicationPathsOperator.Instance;
        public static R5T.F0034.IWindowsExplorerOperator WindowsExplorerOperator => R5T.F0034.WindowsExplorerOperator.Instance;
    }
}
