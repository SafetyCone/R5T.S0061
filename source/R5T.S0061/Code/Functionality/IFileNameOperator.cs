using System;

using R5T.T0132;


namespace R5T.S0061
{
    [FunctionalityMarker]
    public partial interface IFileNameOperator : IFunctionalityMarker,
        F0000.IFileNameOperator
    {
        public string GetOutputFileNameStem_ForInstanceVariety(string instanceVarietyName)
        {
            // Just return the instance variety name.
            var output = instanceVarietyName;
            return output;
        }

        public string GetTextOutputFileName_ForInstanceVariety(string instanceVarietyName)
        {
            var fileNameStem = this.GetOutputFileNameStem_ForInstanceVariety(instanceVarietyName);

            var output = this.GetFileName(
                fileNameStem,
                Instances.FileExtensions.Text);

            return output;
        }
    }
}
