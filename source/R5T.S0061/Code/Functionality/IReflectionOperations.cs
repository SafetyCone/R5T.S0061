using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IReflectionOperations : IFunctionalityMarker
    {
        public InstanceIdentityNames GetTypeInstanceIdentityNames(TypeInfo typeInfo)
        {
            var typeIdentityName = Instances.IdentityNameProvider.GetIdentityName(typeInfo);

            var typeParameterNamedIdentityName = typeIdentityName; // Does not exist yet: Instances.ParameterNamedIdentityNameProvider.GetParameterNamedIdentityName(typeInfo);

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

        /// <summary>
        /// Determines whether the method is a property get or set method.
        /// </summary>
        public bool IsPropertyMethod(MethodInfo methodInfo)
        {
            var output = true
                // All property methods have special names.
                && methodInfo.IsSpecialName
                && methodInfo.DeclaringType.GetProperties()
                    .Any(property => false
                        || property.GetGetMethod() == methodInfo
                        || property.GetSetMethod() == methodInfo);

            return output;
        }

        public bool IsValuesProperty(PropertyInfo propertyInfo)
        {
            var output = true
                // Only properties with get methods.
                && propertyInfo.GetMethod is not null
                // Only properties with public get methods.
                && propertyInfo.GetMethod.IsPublic
                // Only properties *without* set methods.
                && propertyInfo.SetMethod is null
                // Only properties that are *not* indexers (which is tested by seeing if the property has any index parameters).
                && propertyInfo.GetIndexParameters().None()
                ;

            return output;
        }

        public void ForPropertiesOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<PropertyInfo, bool> propertySelector,
            Action<TypeInfo, PropertyInfo> action)
        {
            var propertiesOnTypes = this.SelectPropertiesOnTypes(
                assembly,
                typeSelector,
                propertySelector);

            propertiesOnTypes.ForEach(tuple => action(tuple.TypeInfo, tuple.PropertyInfo));
        }

        public void ForMethodsOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<MethodInfo, bool> methodSelector,
            Action<TypeInfo, MethodInfo> action)
        {
            var methodsOnTypes = this.SelectMethodsOnTypes(
                assembly,
                typeSelector,
                methodSelector);

            methodsOnTypes.ForEach(tuple => action(tuple.TypeInfo, tuple.MethodInfo));
        }

        public IEnumerable<(TypeInfo TypeInfo, MethodInfo MethodInfo)> SelectMethodsOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<MethodInfo, bool> methodSelector)
        {
            var output = F0000.AssemblyOperator.Instance.SelectTypes(assembly, typeSelector)
                .SelectMany(typeInfo => typeInfo.DeclaredMethods
                    .Where(methodSelector)
                    .Select(methodInfo => (typeInfo, methodInfo)));

            return output;
        }

        public IEnumerable<(TypeInfo TypeInfo, PropertyInfo PropertyInfo)> SelectPropertiesOnTypes(
            Assembly assembly,
            Func<TypeInfo, bool> typeSelector,
            Func<PropertyInfo, bool> propertySelector)
        {
            var output = assembly.DefinedTypes
                .Where(typeSelector)
                .SelectMany(typeInfo => typeInfo.DeclaredProperties
                    .Where(propertySelector)
                    .Select(propertyInfo => (typeInfo, propertyInfo)));

            return output;
        }

        public Func<TypeInfo, bool> GetInstanceTypeByMarkerAttributeNamespacedTypeNamePredicate(
            string markerAttributeNamespacedTypeName)
        {
            return F0018.TypeOperator.Instance.GetTypeByHasAttributeOfNamespacedTypeNamePredicate(markerAttributeNamespacedTypeName);
        }
    }
}
