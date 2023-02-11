using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IInstanceOperations : IFunctionalityMarker,
        T001.IInstanceOperations
    {
        /// <summary>
        /// Selects functionality or draft functionality instances.
        /// </summary>
        /// <param name="includeDraft">True to include functionality labeled as draft functionality.</param>
        /// <returns>Functionality or draft functionality instances enumerable requiring iteration.</returns>
        public IEnumerable<InstanceDescriptor> WhereIsFunctionality(
            IEnumerable<InstanceDescriptor> instances,
            bool includeDraft = true)
        {
            var varieties = Instances.EnumerableOperator.From(
                Instances.InstanceVariety.Functionality)
                .AppendIf(
                    includeDraft,
                    Instances.InstanceVariety.DraftFunctionality);

            var output = this.WhereVarietyIn(
                instances,
                varieties);

            return output;
        }
    }
}
