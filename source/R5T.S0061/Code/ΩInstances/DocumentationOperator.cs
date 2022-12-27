using System;


namespace R5T.S0061
{
    public class DocumentationOperator : IDocumentationOperator
    {
        #region Infrastructure

        public static IDocumentationOperator Instance { get; } = new DocumentationOperator();


        private DocumentationOperator()
        {
        }

        #endregion
    }
}
