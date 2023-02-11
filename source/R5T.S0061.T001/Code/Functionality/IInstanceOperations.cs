using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;


namespace R5T.S0061.T001
{
    [FunctionalityMarker]
    public partial interface IInstanceOperations : IFunctionalityMarker
    {
        public IEnumerable<InstanceDescriptor> WhereVarietyIn(
            IEnumerable<InstanceDescriptor> instances,
            HashSet<string> varietyNames)
        {
            var output = instances
                .Where(instance => varietyNames.Contains(instance.InstanceVariety))
                ;

            return output;
        }

        public IEnumerable<InstanceDescriptor> WhereVarietyIn(
            IEnumerable<InstanceDescriptor> instances,
            IEnumerable<string> varietyNames)
        {
            var varietyNamesHash = new HashSet<string>(varietyNames);

            var output = this.WhereVarietyIn(
                instances,
                varietyNamesHash);

            return output;
        }

        public IEnumerable<InstanceDescriptor> WhereVarietyIn(
            IEnumerable<InstanceDescriptor> instances,
            params string[] varietyNames)
        {
            var output = this.WhereVarietyIn(
                instances,
                varietyNames.AsEnumerable());

            return output;
        }

        public IEnumerable<InstanceDescriptor> WhereVarietyIs(
            IEnumerable<InstanceDescriptor> instances,
            string varietyName)
        {
            var output = instances
                .Where(instance => instance.InstanceVariety == varietyName)
                ;

            return output;
        }
    }
}
