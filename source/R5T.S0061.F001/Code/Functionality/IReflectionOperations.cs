using System;
using System.Linq;
using System.Reflection;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IReflectionOperations : IFunctionalityMarker,
        F0018.IReflectionOperations
    {
        public InstanceIdentityNames GetTypeInstanceIdentityNames(TypeInfo typeInfo)
        {
            var typeIdentityName = Instances.IdentityNameProvider.GetIdentityName(typeInfo);

            var typeParameterNamedIdentityName = Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(typeInfo);

            var output = new InstanceIdentityNames
            {
                IdentityName = typeIdentityName,
                ParameterNamedIdentityName = typeParameterNamedIdentityName,
            };

            return output;
        }

        public InstanceIdentityNames GetValuePropertyNames(PropertyInfo propertyInfo)
        {
            var methodIdentityName = Instances.IdentityNameProvider.GetIdentityName(propertyInfo);

            var methodParameterNamedIdentityName = methodIdentityName; // TODO Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(methodInfo);

            var output = new InstanceIdentityNames
            {
                IdentityName = methodIdentityName,
                ParameterNamedIdentityName = methodParameterNamedIdentityName,
            };

            return output;
        }

        public bool IsInstanceMethod(MethodInfo methodInfo)
        {
            var output = true
                // Only public methods.
                && methodInfo.IsPublic
                // Must not be a property.
                && !this.IsPropertyMethod(methodInfo)
                ;

            return output;
        }

        public bool IsValuesProperty(PropertyInfo propertyInfo)
        {
            var output = true
                // Only properties with get methods.
                && propertyInfo.GetMethod is object
                // Only properties with public get methods.
                && propertyInfo.GetMethod.IsPublic
                // Only properties *without* set methods.
                && propertyInfo.SetMethod is null
                // Only properties that are *not* indexers (which is tested by seeing if the property has any index parameters).
                && propertyInfo.GetIndexParameters().None()
                ;

            return output;
        }

        public Func<TypeInfo, bool> GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(
            string markerAttributeNamespacedTypeName)
        {
            return F0018.TypeOperator.Instance.GetTypeByHasAttributeOfNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName);
        }
    }
}
