using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F002
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker
    {
        public string GetInstanceName(InstanceDescriptor instance)
        {
            var name = instance.ParameterNamedIdentityName;
            return name;
        }

        public bool InstanceNameContainsText(string instanceName, string searchTerm)
        {
            var output = instanceName.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant());
            return output;
        }

        public bool InstanceNameContainsText(InstanceDescriptor instance, string searchTerm)
        {
            var instanceName = this.GetInstanceName(instance);

            var containsSearchTerm = this.InstanceNameContainsText(instanceName, searchTerm);
            return containsSearchTerm;
        }

        public IEnumerable<InstanceDescriptor> InstanceNameContainsText(IEnumerable<InstanceDescriptor> instances, string searchTerm)
        {
            var output = instances
                .Where(instance => this.InstanceNameContainsText(instance, searchTerm))
                ;

            return output;
        }

        public InstanceDescriptor[] LoadInstances_Synchronous(string instancesJsonFilePath)
        {
            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(
                instancesJsonFilePath);

            return instances;
        }

        public void SaveInstances_Synchronous(
            string instancesJsonFilePath,
            InstanceDescriptor[] instances)
        {
            Instances.JsonOperator.Serialize_Synchronous(
                instancesJsonFilePath,
                instances);
        }
    }
}
