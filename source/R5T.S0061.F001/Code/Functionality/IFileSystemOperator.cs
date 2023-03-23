using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IFileSystemOperator : IFunctionalityMarker,
        F0000.IFileSystemOperator,
        F0082.IFileSystemOperator,
        F0112.IFileSystemOperator
    {

    }
}
