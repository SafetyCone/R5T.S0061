using System;

using R5T.T0132;


namespace R5T.S0061.F001
{
    [DraftFunctionalityMarker]
    public partial interface IFileNameOperator : IDraftFunctionalityMarker,
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

            var output = this.Get_FileName(
                fileNameStem,
                Instances.FileExtensions.Text);

            return output;
        }

        public string GetPriorToDateFileName(
            string fileName,
            DateTime date)
        {
            var yyyyMMdd = Instances.DateOperator.ToString_YYYYMMDD(date);

            var appendix = $"-Prior to-{yyyyMMdd}";

            var newFileName = this.Append_ToFileNameStem(
                fileName,
                appendix);

            return newFileName;
        }
    }
}
