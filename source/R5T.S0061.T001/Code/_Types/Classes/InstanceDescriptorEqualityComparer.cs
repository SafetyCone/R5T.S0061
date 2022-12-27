using System;
using System.Collections.Generic;


namespace R5T.S0061.T001
{
    public class InstanceDescriptorEqualityComparer : IEqualityComparer<InstanceDescriptor>
    {
        #region Static

        public static InstanceDescriptorEqualityComparer Instance => new InstanceDescriptorEqualityComparer();

        #endregion


        public bool Equals(InstanceDescriptor x, InstanceDescriptor y)
        {
            var output = true
                && x.IdentityName == y.IdentityName
                && x.ProjectFilePath == y.ProjectFilePath
                && x.ParameterNamedIdentityName == y.ParameterNamedIdentityName
                && x.DescriptionXml == y.DescriptionXml
                && x.IdentityName == y.IdentityName
                ;

            return output;
        }

        public int GetHashCode(InstanceDescriptor obj)
        {
            var hashCode = HashCode.Combine(
                obj.InstanceVariety,
                obj.ProjectFilePath,
                // Use identity name, not parameter named identity name, since identity name will be unique in the project and the extra parameter name strings are just extra work.
                obj.ParameterNamedIdentityName);

            // Skip description and parameter named identity name.

            return hashCode;
        }
    }
}
