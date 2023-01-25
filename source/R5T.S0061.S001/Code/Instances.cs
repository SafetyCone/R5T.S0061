using System;


namespace R5T.S0061.S001
{
    public static class Instances
    {
        public static F0000.IArrayOperator ArrayOperator => F0000.ArrayOperator.Instance;
        public static F0000.IEnumerableOperator EnumerableOperator => F0000.EnumerableOperator.Instance;
        public static F0000.IFileOperator FileOperator => F0000.FileOperator.Instance;
        public static IFilePaths FilePaths => S001.FilePaths.Instance;
        public static F0035.ILoggingOperator LoggingOperator => F0035.LoggingOperator.Instance;
        public static F0033.INotepadPlusPlusOperator NotepadPlusPlusOperator => F0033.NotepadPlusPlusOperator.Instance;
        public static IOperations Operations => S001.Operations.Instance;
        public static IScripts Scripts => S001.Scripts.Instance;
    }
}