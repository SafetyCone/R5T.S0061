using System;

using R5T.T0131;


namespace R5T.S0061.F001
{
    [DraftValuesMarker]
    public partial interface IDirectoryPaths : IDraftValuesMarker,
        Z0026.IDirectoryPaths
    {
        public string NetCoreApp_RuntimesDirectoryPath => @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\";
        public string AspNetCoreApp_RuntimesDirectoryPath => @"C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\";
        public string WindowsDesktopApp_RuntimesDirectoryPath => @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\";
    }
}
