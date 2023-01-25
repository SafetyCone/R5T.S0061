using System;

using R5T.T0131;


namespace R5T.S0061
{
    [ValuesMarker]
    public partial interface IValues : IValuesMarker,
        F001.IValues
    {
        public string ApplicationName => "R5T.S0061";
    }
}
