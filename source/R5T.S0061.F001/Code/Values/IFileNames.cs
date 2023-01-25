using System;

using R5T.T0131;


namespace R5T.S0061.F001
{
    [ValuesMarker]
    public partial interface IFileNames : IValuesMarker
    {
        public string BuildJsonFileName => "R5T.S0061.Build.json";
    }
}
