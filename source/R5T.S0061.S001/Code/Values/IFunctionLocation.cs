using System;

using R5T.T0131;


namespace R5T.S0061.S001
{
    [ValuesMarker]
    public partial interface IFunctionLocation : IValuesMarker
    {
        public string InstanceName => "InstanceName";
        public string MethodName => "MethodName";
        public string ArgumentName => "ArgumentName";
        public string ProjectName => "ProjectName";
        public string ArgumentTypeName => "ArgumentTypeName";
        public string OutputTypeName => "OutputTypeName";
        public string Comments => "Comments";
    }
}
