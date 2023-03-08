using System;


namespace R5T.S0061.S001
{
    public class Searches : ISearches
    {
        #region Infrastructure

        public static ISearches Instance { get; } = new Searches();


        private Searches()
        {
        }

        #endregion
    }
}
