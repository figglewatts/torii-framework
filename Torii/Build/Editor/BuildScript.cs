using System.Collections.Generic;
using Torii.Serialization;
using Torii.Util;
using UnityEditor;
using UnityEngine;

namespace Torii.Build
{
    /// <summary>
    /// BuildScript is used to build the game when running Unity from the command line.
    /// </summary>
    public class BuildScript
    {
        private static readonly ToriiSerializer _serializer = new ToriiSerializer();

        /// <summary>
        /// The filename of the build definition file to load.
        /// </summary>
        private const string BUILD_DEFINITION_FILE = "build_defs.json";

        /// <summary>
        /// Used for building the game.
        /// </summary>
        public static void Build()
        {
            Debug.Log("Loading build definitions...");

            // the build definition file is stored in the project folder, one above the 'Assets/' folder
            string buildDefinitionPath = PathUtil.Combine(Application.dataPath, "../", BUILD_DEFINITION_FILE);

            // load the build definitions
            var buildDefs = _serializer.JsonDeserialize<List<BuildDefinition>>(buildDefinitionPath);

            foreach (var buildDef in buildDefs)
            {
                Debug.Log($"Building player for target '{buildDef.Target}'...");

                var result = BuildPipeline.BuildPlayer(buildDef.ToBuildPlayerOptions());

                Debug.Log(result);
            }
        }
    }
}
