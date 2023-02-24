using System;

using R5T.T0132;


namespace R5T.S0061
{
	[FunctionalityMarker]
	public partial interface IInstanceVarietyOperator : IFunctionalityMarker,
		F001.IInstancesVarietyOperator
	{
		public string GetInstanceVarietyName(string markerAttributeNamespacedTypeName)
        {
			// Use the type name of the marker attribute.
			var instanceVarietyName = F0000.Instances.NamespacedTypeNameOperator.GetTypeName(markerAttributeNamespacedTypeName);
			return instanceVarietyName;
        }
	}
}