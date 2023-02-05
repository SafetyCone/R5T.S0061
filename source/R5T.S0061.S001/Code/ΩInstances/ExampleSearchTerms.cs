using System;


namespace R5T.S0061.S001
{
    public class ExampleSearchTerms : IExampleSearchTerms
    {
        #region Infrastructure

        public static IExampleSearchTerms Instance { get; } = new ExampleSearchTerms();


        private ExampleSearchTerms()
        {
        }

        #endregion
    }
}
