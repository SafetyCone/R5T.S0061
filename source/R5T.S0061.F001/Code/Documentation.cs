using System;


namespace R5T.S0061.F001
{
	/// <summary>
	/// Functionality library for the Fresno script.
	/// </summary>
	public static class Documentation
	{
        /// <summary>
        /// A list of projects that have failed to build.
        /// </summary>
        public static readonly object BuildProblemProjects;

        /// <summary>
        /// A list of projects that have been successfully built, but failed processing.
        /// </summary>
        public static readonly object ProcessingProblemProjects;

        /// <summary>
        /// A list of project file paths for which a build should not be attempted.
        /// </summary>
        /// <remarks>
        /// Some projects are just hopeless. They might be too old, require updates that are too involved for now, or are just irrelevant (like the _NuGet-specific projects).
        /// To avoid wasting time trying to build these projects, put the project file paths in this file and they will be filtered out of the <see cref="ProjectsList_AllTextFilePath"/> to make the <see cref="ProjectsListTextFilePath"/>
        /// This file should be centralized somewhere as a TODO list of projects to evaluated.
        /// </remarks>
        public static readonly object DoNotBuildProjectsListTextFilePath;

        /// <summary>
        /// The central source for what project file paths are of interest.
        /// The contents of this file have been filtered using the <see cref="DoNotBuildProjectsListTextFilePath"/> on the <see cref="ProjectsList_AllTextFilePath"/>.
        /// </summary>
        /// <remarks>
        /// The list of projects that are processed (built and analyzed) is determined by the contents of the projects list file.
        /// This allows the script to be divided into two phases:
        ///     1. Searching for and filtering project file paths of interest.
        ///     2. Processing project file paths of interest.
        /// This file serves as an explicit checkpoint between the two phases.
        /// </remarks>
        public static readonly object ProjectsListTextFilePath;

        /// <summary>
        /// A file containing <em>all</em> project file paths that are found.
        /// The contents of this file are filtered using the <see cref="DoNotBuildProjectsListTextFilePath"/> to get the <see cref="ProjectsListTextFilePath"/>.
        /// </summary>
        public static readonly object ProjectsList_AllTextFilePath;

        /// <summary>
        /// Contains all instances.
        /// This is the single source of truth for what instances exist, and is the result of comparing the per-run instances (<see cref="Instances_PerRun"/>) with a prior instances file.
        /// </summary>
        public static readonly object Instances;

        /// <summary>
        /// Contains the output instances from a run.
        /// It needs to be compared to a prior instances file (<see cref="Instances"/>) using the <see cref="BuildProblemProjects"/> and <see cref="ProcessingProblemProjects"/> lists to avoid remove instances that still exist, but have failed building or processing during this run.
        /// </summary>
        public static readonly object Instances_PerRun;

        /// <summary>
        /// Compare the instances found in today's run with the full set of instances accumulated over time.
        /// Returns the set of new instances (in today's run but not in the prior set), missing instances (not in today's run, but in the prior set), and most importantly, removed instances: missing instances whose containing project was not in the build problems or processing problems project lists.
        /// The distinction between missing and removed instances is important since instances might be missing from today's run only because their containing projects failed to build or process.
        /// These instances should not be removed since we want to know of their existence.
        /// Thus we only remove instances that no longer exist in projects that have successfully built and processed (i.e. were actually removed).
        /// </summary>
        public static readonly object For_CompareRunInstances;
    }
}