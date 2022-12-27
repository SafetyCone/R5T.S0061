using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using R5T.T0132;


namespace R5T.S0061
{
    /// <summary>
    /// Provides methods to get a parameter-named identity name (an identity name where the names of both parameters and type parameters are included).
    /// </summary>
    [DraftFunctionalityMarker]
    public interface IParameterNamedIdentityNameProvider : IDraftFunctionalityMarker
    {
        public string GetNamespacedTypeName(Type type)
        {
            var output = $"{type.Namespace}.{type.Name}";
            return output;
        }

        public string GetTypeParametersToken(IEnumerable<Type> typeParameters)
        {
            var anyTypeParameters = typeParameters.Any();

            var output = anyTypeParameters
                ? "<" + String.Join(", ", typeParameters
                    .Select(xMethodTypeParameter => xMethodTypeParameter.Name)) + ">"
                : String.Empty
                ;

            return output;
        }

        public string GetParameterNamedIdentityName(MethodInfo methodInfo)
        {
            var declaringTypeParameterNamedIdentityNameValue = this.GetParameterNamedIdentityNameValue(methodInfo.DeclaringType);

            var methodNameToken = methodInfo.Name;

            var methodTypeParameters = methodInfo.GetGenericArguments();

            var methodTypeParametersToken = this.GetTypeParametersToken(methodTypeParameters);

            var parameters = methodInfo.GetParameters();

            var parametersToken = parameters.Any()
                ? "(" + String.Join(", ", parameters
                        .Select(xParameter => this.GetParameterTokenForMethodIdentityName(xParameter))) + ")"
                : String.Empty;
            ;

            var output = $"M:{declaringTypeParameterNamedIdentityNameValue}.{methodNameToken}{methodTypeParametersToken}{parametersToken}";
            return output;
        }

        public string GetParameterNamedIdentityNameValue(Type type)
        {
            var typeTypeParameters = type.GetGenericArguments();

            var typeTypeParametersToken = this.GetTypeParametersToken(typeTypeParameters);

            var namespacedTypeName = this.GetNamespacedTypeName(type);

            var output = $"{namespacedTypeName}{typeTypeParametersToken}";
            return output;
        }

        public string GetParameterTokenForMethodIdentityName(ParameterInfo parameter)
        {
            var parameterTypeName = this.GetParameterTypeNameForMethodIdentityName(parameter.ParameterType);

            var output = $"{parameterTypeName} {parameter.Name}";
            return output;
        }

        public string GetParameterTypeNameForMethodIdentityName(Type parameterType)
        {
            if (parameterType.IsGenericType)
            {
                // Keep only the portion up to the generic type parameter arity token separator.
                var adjustedName = parameterType.Name.Split('`').First();

                var namespacedTypeName = $"{parameterType.Namespace}.{adjustedName}";

                var genericArguments = parameterType.GetGenericArguments();

                var argumentsToken = String.Join(", ", genericArguments
                    // Recurse.
                    .Select(xGenericArgumentType => this.GetParameterTypeNameForMethodIdentityName(xGenericArgumentType)));

                var output = $"{namespacedTypeName}{{{argumentsToken}}}";
                return output;
            }

            if(parameterType.IsGenericTypeParameter || parameterType.IsGenericMethodParameter)
            {
                return parameterType.Name;
            }

            return parameterType.FullName;
        }
    }
}
