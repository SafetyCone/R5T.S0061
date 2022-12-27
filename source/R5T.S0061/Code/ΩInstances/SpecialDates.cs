using System;


namespace R5T.S0061
{
    public class SpecialDates : ISpecialDates
    {
        #region Infrastructure

        public static ISpecialDates Instance { get; } = new SpecialDates();


        private SpecialDates()
        {
        }

        #endregion
    }
}
