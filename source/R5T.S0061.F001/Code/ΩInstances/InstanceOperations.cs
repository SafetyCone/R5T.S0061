using System;


namespace R5T.S0061.F001
{
    public class InstanceOperations : IInstanceOperations
    {
        #region Infrastructure

        public static IInstanceOperations Instance { get; } = new InstanceOperations();


        private InstanceOperations()
        {
        }

        #endregion
    }
}
