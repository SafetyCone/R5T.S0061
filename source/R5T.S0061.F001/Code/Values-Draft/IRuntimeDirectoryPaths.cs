using System;

using R5T.T0131;


namespace R5T.S0061.F001
{
    [DraftValuesMarker]
    public partial interface IRuntimeDirectoryPaths : IDraftValuesMarker
    {
        public string AspNetCore_5 => @"C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\5.0.17\";
        public string AspNetCore_6 => @"C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\6.0.13\";
        public string AspNetCore => this.AspNetCore_6;
        public string NetCore_5 => @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.17\";
        public string NetCore_6 => @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.13\";
        public string NetCore => this.NetCore_6;
        public string WindowsFormsNetCore_5 => @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\5.0.17\";
        public string WindowsFormsNetCore_6 => @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.13\";
        public string WindowsFormsNetCore => this.WindowsFormsNetCore_6;
    }
}
