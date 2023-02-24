using System;


namespace R5T.S0061.F001
{
    public class InstanceSetComparisonOperator : IInstanceSetComparisonOperator
    {
        #region Infrastructure

        public static IInstanceSetComparisonOperator Instance { get; } = new InstanceSetComparisonOperator();


        private InstanceSetComparisonOperator()
        {
        }

        #endregion
    }
}
