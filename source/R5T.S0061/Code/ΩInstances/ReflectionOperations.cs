using System;


namespace R5T.S0061
{
    public class ReflectionOperations : IReflectionOperations
    {
        #region Infrastructure

        public static IReflectionOperations Instance { get; } = new ReflectionOperations();


        private ReflectionOperations()
        {
        }

        #endregion
    }
}
