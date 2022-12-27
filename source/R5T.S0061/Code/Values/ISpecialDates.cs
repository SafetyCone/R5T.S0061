using System;

using R5T.T0131;


namespace R5T.S0061
{
    [ValuesMarker]
    public partial interface ISpecialDates : IValuesMarker
    {
        /// <summary>
		/// When running the date-to-date survey comparison code for the first time, there is no prior comparison date.
		/// The comparison code has been implemented such that if a dated file path does not exist, an empty set of functionality descriptors is returned.
		/// Thus any date will work.
		/// </summary>
		public DateTime DefaultPriorComparisonDate => DateTime.MinValue;
    }
}
