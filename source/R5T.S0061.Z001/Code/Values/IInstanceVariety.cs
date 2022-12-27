using System;

using R5T.T0131;


namespace R5T.S0061.Z001
{
    [ValuesMarker]
    public partial interface IInstanceVariety : IValuesMarker
    {
        public string DraftMarkerAttribute => "Marker Attributes-Draft";
        public string MarkerAttribute => "Marker Attributes";

        public string DraftConstants => "Constants-Draft-OBSOLETE";
        public string DraftContextType => "Context Type-Draft";
        public string DraftDataType => "Data Type-Draft";
        public string DraftDemonstrations => "Demonstrations-Draft";
        public string DraftExperiments => "Experiments-Draft";
        public string DraftExplorations => "Explorations-Draft";
        public string DraftFunctionality => "Functionality-Draft";
        public string DraftStrongType => "Strong Type-Draft";
        public string DraftType => "Type-Draft";
        public string DraftUtilityType => "Utility Type-Draft";
        public string DraftValues => "Values-Draft";

        public string Constants => "Constants-OBSOLETE";
        public string ContextDefinition => "Context Definition";
        public string ContextImplementation => "Context Implementation";
        public string ContextType => "Context Type";
        public string DataType => "Data Type";
        public string Demonstrations => "Demonstrations";
        public string Experiments => "Experiments";
        public string Explorations => "Explorations";
        public string Functionality => "Functionality";
        public string StrongType => "Strong Type";
        public string Type => "Type";
        public string UtilityType => "Utility Type";
        public string Values => "Values";

        public string DraftServiceDefinitions => "Service Definitions-Draft";
        public string DraftServiceImplementations => "Service Implementations-Draft";

        public string ServiceDefinitions => "Service Definitions";
        public string ServiceImplementations => "Service Implementations";
    }
}
