using System;


namespace R5T.S0061.F001
{
    public class InstanceVarietyOperator : IInstanceVarietyOperator
    {
        #region Infrastructure

        public static IInstanceVarietyOperator Instance { get; } = new InstanceVarietyOperator();


        private InstanceVarietyOperator()
        {
        }

        #endregion
    }
}
