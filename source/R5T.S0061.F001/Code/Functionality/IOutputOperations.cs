using System;
using System.Collections.Generic;
using System.Linq;

using R5T.T0132;

using R5T.S0061.T001;


namespace R5T.S0061.F001
{
    [FunctionalityMarker]
    public partial interface IOutputOperations : IFunctionalityMarker
    {
        public string[] OutputInstanceSpecificFiles(
            string instancesJsonFilePath)
        {
            /// Inputs.
            var date = Instances.NowOperator.Get_Today();


            /// Run.
            var datedOutputDirectoryPath = Instances.DirectoryPathOperator.Get_DatedOutputDirectoryPath(date);

            var instances = Instances.JsonOperator.Deserialize_Synchronous<InstanceDescriptor[]>(instancesJsonFilePath);
            var instancesByVariety = instances
                .GroupBy(x => x.InstanceVariety)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());

            var instanceVarietyNames = Instances.InstancesVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder();

            var outputFilePaths = new List<string>(instanceVarietyNames.Length);

            foreach (var instanceVarietyName in instanceVarietyNames)
            {
                var fileName = Instances.FileNameOperator.GetTextOutputFileName_ForInstanceVariety(instanceVarietyName);

                var outputFilePath = Instances.PathOperator.Get_FilePath(
                    datedOutputDirectoryPath,
                    fileName);

                outputFilePaths.Add(outputFilePath);

                var instancesOfVariety = instancesByVariety.ContainsKey(instanceVarietyName)
                    ? instancesByVariety[instanceVarietyName]
                    : Array.Empty<InstanceDescriptor>()
                    ;

                var title = instanceVarietyName;

                var lines = Instances.EnumerableOperator.From($"{title}, Count: {instancesOfVariety.Length}\n\n")
                    .Append(instancesOfVariety
                        .GroupBy(x => x.ProjectFilePath)
                        .OrderAlphabeticallyBy(x => x.Key)
                        .SelectMany(xGroup => Instances.EnumerableOperator.From($"{xGroup.Key}:")
                            .Append(xGroup
                                // Order by the identity name.
                                .OrderAlphabeticallyBy(x => x.IdentityName)
                                // But output the parameter named identity name.
                                .Select(x => $"\t{x.ParameterNamedIdentityName}")
                                .Append(Instances.Strings.Empty))));

                Instances.FileOperator.WriteAllLines_Synchronous(
                    outputFilePath,
                    lines);
            }

            return outputFilePaths.ToArray();
        }
    }
}
