using System;

using R5T.T0131;


namespace R5T.S0061.F001
{
    [DraftValuesMarker]
    public partial interface IRuntimeDirectoryPaths : IDraftValuesMarker
    {
        public string AspNetCore => @"C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\6.0.11\";
        public string NetCore => @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.11\";
        public string WindowsFormsNetCore => @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.11\";
    }
}
