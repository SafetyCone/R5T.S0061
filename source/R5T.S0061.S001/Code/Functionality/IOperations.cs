using System;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.S001
{
    [FunctionalityMarker]
    public partial interface IOperations : IFunctionalityMarker,
        F001.IOperations,
        F002.IOperations
    {
    }
}
