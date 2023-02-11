using System;


namespace R5T.S0061.S001
{
    public class FunctionLocation : IFunctionLocation
    {
        #region Infrastructure

        public static IFunctionLocation Instance { get; } = new FunctionLocation();


        private FunctionLocation()
        {
        }

        #endregion
    }
}
