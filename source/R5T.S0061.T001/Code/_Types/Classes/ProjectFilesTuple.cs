using System;

using R5T.T0142;


namespace R5T.S0061
{
    [DataTypeMarker]
    public class ProjectFilesTuple
    {
        public string ProjectFilePath { get; set; }
        public string AssemblyFilePath { get; set; }
        public string DocumentationFilePath { get; set; }
    }
}
