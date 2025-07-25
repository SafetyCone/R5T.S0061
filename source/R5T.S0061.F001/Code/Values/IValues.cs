using System;
using System.Collections.Generic;

using R5T.T0131;


namespace R5T.S0061.F001
{
    [ValuesMarker]
    public partial interface IValues : IValuesMarker
    {
        public string ApplicationName => "R5T.S0061";

        /// <summary>
        /// Instance varieties for which we want the methods of the type.
        /// </summary>
        public Dictionary<string, string> MethodNameMarkerAttributeNamespacedTypeNamesByInstanceVariety => new Dictionary<string, string>()
        {
            { Instances.InstanceVariety.Functionality, Instances.NamespacedTypeNames.FunctionalityMarkerAttribute },
            { Instances.InstanceVariety.DraftFunctionality, Instances.NamespacedTypeNames.DraftFunctionalityMarkerAttribute },
            { Instances.InstanceVariety.Explorations, Instances.NamespacedTypeNames.ExplorationsMarkerAttribute },
            { Instances.InstanceVariety.DraftExplorations, Instances.NamespacedTypeNames.DraftExplorationsMarkerAttribute },
            { Instances.InstanceVariety.Experiments, Instances.NamespacedTypeNames.ExperimentsMarkerAttribute },
            { Instances.InstanceVariety.DraftExperiments, Instances.NamespacedTypeNames.DraftExperimentsMarkerAttribute },
            { Instances.InstanceVariety.Demonstrations, Instances.NamespacedTypeNames.DemonstrationsMarkerAttribute },
            { Instances.InstanceVariety.DraftDemonstrations, Instances.NamespacedTypeNames.DraftDemonstrationsMarkerAttribute },
        };

        /// <summary>
        /// Instance varieties for which we want the properties of the type.
        /// </summary>
        public Dictionary<string, string> PropertyNameMarkerAttributeNamespacedTypeNamesByInstanceVariety => new Dictionary<string, string>()
        {
            { Instances.InstanceVariety.Values, Instances.NamespacedTypeNames.ValuesMarkerAttribute },
            { Instances.InstanceVariety.DraftValues, Instances.NamespacedTypeNames.DraftValuesMarkerAttribute },
            { Instances.InstanceVariety.Constants, Instances.NamespacedTypeNames.ConstantsMarkerAttribute },
            { Instances.InstanceVariety.DraftConstants, Instances.NamespacedTypeNames.DraftsConstantsMarkerAttribute },
        };

        /// <summary>
        /// Instances varieties for which we want the type itself.
        /// </summary>
        public Dictionary<string, string> InstanceTypeMarkerAttributeNamespacedTypeNamesByVarietyName => new Dictionary<string, string>()
        {
            { Instances.InstanceVariety.MarkerAttribute, F0000.Instances.TypeOperator.Get_NamespacedTypeName<T0143.MarkerAttributeMarkerAttribute>() },
            { Instances.InstanceVariety.DraftMarkerAttribute, F0000.Instances.TypeOperator.Get_NamespacedTypeName<T0143.DraftMarkerAttributeMarkerAttribute>() },
            { Instances.InstanceVariety.ContextDefinition, Instances.NamespacedTypeNames.ContextDefinitionMarkerAttribute },
            { Instances.InstanceVariety.ContextImplementation, Instances.NamespacedTypeNames.ContextImplementationMarkerAttribute },
            { Instances.InstanceVariety.ContextType, Instances.NamespacedTypeNames.ContextTypeMarkerAttribute },
            { Instances.InstanceVariety.DraftContextType, Instances.NamespacedTypeNames.DraftContextTypeMarkerAttribute },
            { Instances.InstanceVariety.DataType, Instances.NamespacedTypeNames.DataTypeMarkerAttribute },
            { Instances.InstanceVariety.DraftDataType, Instances.NamespacedTypeNames.DraftDataTypeMarkerAttribute },
            { Instances.InstanceVariety.UtilityType, Instances.NamespacedTypeNames.UtilityTypeMarkerAttribute },
            { Instances.InstanceVariety.DraftUtilityType, Instances.NamespacedTypeNames.DraftUtilityTypeMarkerAttribute },
            { Instances.InstanceVariety.StrongType, Instances.NamespacedTypeNames.StrongTypeMarkerAttribute},
            { Instances.InstanceVariety.DraftStrongType, Instances.NamespacedTypeNames.DraftStrongTypeMarkerAttribute },
            { Instances.InstanceVariety.Type, Instances.NamespacedTypeNames.TypeMarkerAttribute },
            { Instances.InstanceVariety.DraftType, Instances.NamespacedTypeNames.DraftTypeMarkerAttribute },

            { Instances.InstanceVariety.ServiceDefinitions, Instances.NamespacedTypeNames.ServiceDefinitionMarkerAttribute },
            { Instances.InstanceVariety.ServiceImplementations, Instances.NamespacedTypeNames.ServiceImplementationMarkerAttribute },
            { Instances.InstanceVariety.DraftServiceDefinitions, Instances.NamespacedTypeNames.DraftServiceDefinitionMarkerAttribute },
            { Instances.InstanceVariety.DraftServiceImplementations, Instances.NamespacedTypeNames.DraftServiceImplementationMarkerAttribute },

            { Instances.InstanceVariety.RazorComponents, Instances.NamespacedTypeNames.RazorComponentMarkerAttribute },
            { Instances.InstanceVariety.DraftRazorComponents, Instances.NamespacedTypeNames.DraftRazorComponentMarkerAttribute },
        };
    }
}
