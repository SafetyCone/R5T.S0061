using System;

using R5T.T0142;


namespace R5T.S0061
{
    /// <summary>
    /// A tuple containing a project file path, built assembly file path for that project, and the generated documentation file path for that project.
    /// </summary>
    [DataTypeMarker]
    public class ProjectFileTuple
    {
        public string ProjectFilePath { get; set; }
        public string AssemblyFilePath { get; set; }
        public string DocumentationFilePath { get; set; }
    }
}
