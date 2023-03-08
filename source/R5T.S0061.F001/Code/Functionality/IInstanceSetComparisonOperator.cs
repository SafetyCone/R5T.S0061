using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IInstanceSetComparisonOperator : IFunctionalityMarker
    {
        /// <summary>
        /// <inheritdoc cref="Documentation.For_CompareRunInstances" path="/summary"/>
        /// </summary>
        /// <param name="runInstances">The instances from today's run.</param>
        /// <param name="priorToTodayInstances">The full set of instances accumulated over time.</param>
        /// <param name="buildProblemProjectFilePaths">The list of projects that failed to build during today's run.</param>
        /// <param name="processingProblemProjectFilePaths">The list of projects that failed to process during today's run.</param>
        public (
            InstanceDescriptor[] newInstances,
            InstanceDescriptor[] removedInstances,
            InstanceDescriptor[] missingInstances)
        CompareRunInstances(
            IList<InstanceDescriptor> runInstances,
            IList<InstanceDescriptor> priorToTodayInstances,
            // Do not include "do not build" projects, so that any instances in them are advertised forever.
            IList<string> buildProblemProjectFilePaths,
            IList<string> processingProblemProjectFilePaths)
        {
            // Determine added instances: these are easy, it's just what instances exist in the "per-run" file that don't exist in the "prior-to" today file.
            var newInstances = runInstances.Except(
                priorToTodayInstances,
                T001.N002.InstanceDescriptorEqualityComparer.Instance)
                .Now();

            // Determine removed instances: these are harder.
            //  First determine what instances exist in the "prior-to" today file that do not exist in the "per-run" file.
            //  Then load the build problems and processing problems file paths.
            //  For any instances that are in projects in either of the build problems or processing problems files, remove them from the list.
            // Whatever instances remain, those instances have actually been removed.
            var missingInstances = priorToTodayInstances.Except(
                runInstances,
                T001.N002.InstanceDescriptorEqualityComparer.Instance)
                .Now();

            // Note: do not include "do not build" projects, since those instances should be visible forever (if they exist), as advertisements for processing those projects.

            var projectsToIgnoreFilePathsHash = new HashSet<string>()
                .AddRange(buildProblemProjectFilePaths)
                .AddRange(processingProblemProjectFilePaths);

            var removedInstances = missingInstances
                .Where(instance =>
                {
                    var ignoreProject = projectsToIgnoreFilePathsHash.Contains(
                        instance.ProjectFilePath);

                    var output = !ignoreProject;
                    return output;
                })
                .Now();

            return (newInstances, removedInstances, missingInstances);
        }

        /// <summary>
        /// <inheritdoc cref="Documentation.For_CompareRunInstances" path="/summary"/>
        /// </summary>
        /// <param name="runInstancesJsonFilePath">JSON file containing instances from today's run.</param>
        /// <param name="priorToTodayInstancesJsonFilePath">JSON file containing instances contaiing the full set of instances accumulated over time.</param>
        /// <param name="buildProblemProjectsTextFilePath">Text file containing the list of projects that failed to build during today's run.</param>
        /// <param name="processingProblemProjectsTextFilePath">Text file containing the list of projects that failed to process during today's run.</param>
        public (
            InstanceDescriptor[] newInstances,
            InstanceDescriptor[] removedInstances,
            InstanceDescriptor[] missingInstances)
        CompareRunInstances(
            string runInstancesJsonFilePath,
            string priorToTodayInstancesJsonFilePath,
            // Do not include "do not build" projects, so that any instances in them are advertised forever.
            string buildProblemProjectsTextFilePath,
            string processingProblemProjectsTextFilePath)
        {
            // Load the "prior-to" today's date instances file, and the "per-run" instances file.
            var priorToTodayInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(
                priorToTodayInstancesJsonFilePath);

            var runInstances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(
                runInstancesJsonFilePath);

            var buildProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(
                buildProblemProjectsTextFilePath);

            var processingProblemProjectFilePaths = Instances.FileOperator.ReadAllLines_Synchronous(
                buildProblemProjectsTextFilePath);

            return this.CompareRunInstances(
                runInstances,
                priorToTodayInstances,
                buildProblemProjectFilePaths,
                processingProblemProjectFilePaths);
        }
    }
}
