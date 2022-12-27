using System;


namespace R5T.S0061.Z001
{
    public class InstanceVariety : IInstanceVariety
    {
        #region Infrastructure

        public static IInstanceVariety Instance { get; } = new InstanceVariety();


        private InstanceVariety()
        {
        }

        #endregion
    }
}
