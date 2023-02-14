using System;
using System.Collections.Generic;


namespace R5T.S0061.T001
{
    public class ProjectDependenciesSet
    {
        /// <summary>
        /// Keep track of all recursive dependencies of a project, exclusive of the project itself (which is available as the key).
        /// </summary>
        public Dictionary<string, string[]> RecursiveProjectDependenciesByProjectFilePath_Exclusive { get; } = new Dictionary<string, string[]>();

        /// <summary>
        /// Keep track of which projects have an ASP.NET Core framework dependency.
        /// This is used for determining which dependency assemblies to add during reflection on assembly metadata.
        /// </summary>
        public Dictionary<string, bool> HasAspNetDependencyByProjectFilePath { get; } = new Dictionary<string, bool>();
    }
}
