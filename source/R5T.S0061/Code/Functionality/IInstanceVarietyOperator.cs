using System;

using R5T.T0132;


namespace R5T.S0061
{
	[FunctionalityMarker]
	public partial interface IInstanceVarietyOperator : IFunctionalityMarker
	{
		public string[] GetAllInstanceVarietyNames_InPresentationOrder()
        {
			var output = new[]
			{
				Instances.InstanceVariety.Functionality,
				Instances.InstanceVariety.Values,
				Instances.InstanceVariety.DataType,
				Instances.InstanceVariety.StrongType,
				Instances.InstanceVariety.UtilityType,
				Instances.InstanceVariety.DraftFunctionality,
				Instances.InstanceVariety.DraftValues,
				Instances.InstanceVariety.DraftDataType,
				Instances.InstanceVariety.DraftStrongType,
				Instances.InstanceVariety.DraftUtilityType,
				Instances.InstanceVariety.MarkerAttribute,
				Instances.InstanceVariety.DraftMarkerAttribute,
				Instances.InstanceVariety.Explorations,
				Instances.InstanceVariety.DraftExplorations,
				Instances.InstanceVariety.Experiments,
				Instances.InstanceVariety.DraftExperiments,
				Instances.InstanceVariety.Demonstrations,
				Instances.InstanceVariety.DraftDemonstrations,
				Instances.InstanceVariety.Constants,
				Instances.InstanceVariety.DraftConstants,
				Instances.InstanceVariety.ContextDefinition,
				Instances.InstanceVariety.ContextImplementation,
				Instances.InstanceVariety.ContextType,
				Instances.InstanceVariety.DraftContextType,

				Instances.InstanceVariety.ServiceDefinitions,
				Instances.InstanceVariety.ServiceImplementations,
				Instances.InstanceVariety.DraftServiceDefinitions,
				Instances.InstanceVariety.DraftServiceImplementations,
			};

			return output;
		}

		public string[] GetAllInstanceVarietyNames()
        {
			var instanceVarietyValues = Instances.InstanceVariety;

			var output = new[]
			{
				instanceVarietyValues.Constants,
				instanceVarietyValues.ContextDefinition,
				instanceVarietyValues.ContextImplementation,
				instanceVarietyValues.ContextType,
				instanceVarietyValues.DataType,
				instanceVarietyValues.Demonstrations,
				instanceVarietyValues.DraftConstants,
				instanceVarietyValues.DraftContextType,
				instanceVarietyValues.DraftDataType,
				instanceVarietyValues.DraftDemonstrations,
				instanceVarietyValues.DraftExperiments,
				instanceVarietyValues.DraftExplorations,
				instanceVarietyValues.DraftFunctionality,
				instanceVarietyValues.DraftMarkerAttribute,
				instanceVarietyValues.DraftServiceDefinitions,
				instanceVarietyValues.DraftServiceImplementations,
				instanceVarietyValues.DraftStrongType,
				instanceVarietyValues.DraftUtilityType,
				instanceVarietyValues.DraftValues,
				instanceVarietyValues.Experiments,
				instanceVarietyValues.Explorations,
				instanceVarietyValues.Functionality,
				instanceVarietyValues.MarkerAttribute,
				instanceVarietyValues.ServiceDefinitions,
				instanceVarietyValues.ServiceImplementations,
				instanceVarietyValues.StrongType,
				instanceVarietyValues.UtilityType,
				instanceVarietyValues.Values,
			};

			return output;
        }

		public string GetInstanceVarietyName(string markerAttributeNamespacedTypeName)
        {
			// Use the type name of the marker attribute.
			var instanceVarietyName = F0000.Instances.NamespacedTypeNameOperator.GetTypeName(markerAttributeNamespacedTypeName);
			return instanceVarietyName;
        }
	}
}