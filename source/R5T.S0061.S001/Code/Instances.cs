using System;


namespace R5T.S0061.S001
{
    public static class Instances
    {
        public static F0000.IArrayOperator ArrayOperator => F0000.ArrayOperator.Instance;
        public static F0000.IDateOperator DateOperator => F0000.DateOperator.Instance;
        public static F001.IDirectoryPathOperator DirectoryPathOperator => F001.DirectoryPathOperator.Instance;
        public static F0103.IDurationFormatters DurationFormatters => F0103.DurationFormatters.Instance;
        public static F0000.IEnumerableOperator EnumerableOperator => F0000.EnumerableOperator.Instance;
        public static F001.IFilePathOperator FilePathOperator => F001.FilePathOperator.Instance;
        public static F0000.IFileOperator FileOperator => F0000.FileOperator.Instance;
        public static F001.IFileNameOperator FileNameOperator => F001.FileNameOperator.Instance;
        public static IFilePaths FilePaths => S001.FilePaths.Instance;
        public static F0000.IFileSystemOperator FileSystemOperator => F0000.FileSystemOperator.Instance;
        public static IFunctionLocation FunctionLocation => S001.FunctionLocation.Instance;
        public static T0158.F000.IHumanOutputTextFilePathOperator HumanOutputTextFilePathOperator => T0158.F000.HumanOutputTextFilePathOperator.Instance;
        public static F001.IInstanceOperations InstanceOperations => F001.InstanceOperations.Instance;
        public static F001.IInstanceSetComparisonOperator InstanceSetComparisonOperator => F001.InstanceSetComparisonOperator.Instance;
        public static F001.IInstanceVarietyOperator InstancesVarietyOperator => F001.InstanceVarietyOperator.Instance;
        public static F0032.IJsonOperator JsonOperator => F0032.JsonOperator.Instance;
        public static F0035.ILoggingOperator LoggingOperator => F0035.LoggingOperator.Instance;
        public static T0148.ILogFilePathOperator LogFilePathOperator => T0148.LogFilePathOperator.Instance;
        public static F0033.INotepadPlusPlusOperator NotepadPlusPlusOperator => F0033.NotepadPlusPlusOperator.Instance;
        public static F0000.INowOperator NowOperator => F0000.NowOperator.Instance;
        public static IOperations Operations => S001.Operations.Instance;
        public static F001.IOutputOperations OutputOperations => F001.OutputOperations.Instance;
        public static F0002.IPathOperator PathOperator => F0002.PathOperator.Instance;
        public static IScripts Scripts => S001.Scripts.Instance;
        public static T0159.F000.ITextOutputOperator TextOutputOperator => T0159.F000.TextOutputOperator.Instance;
        public static F0103.ITimingOperator TimingOperator => F0103.TimingOperator.Instance;
    }
}