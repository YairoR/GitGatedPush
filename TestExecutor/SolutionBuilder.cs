﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace TestExecutor
{
    /// <summary>
    /// Tool for building a given solution using the MSBuild component.
    /// </summary>
    public class SolutionBuilder
    {
        /// <summary>
        /// Start to build given solution path.
        /// </summary>
        /// <param name="solutionPath">The solution's path.</param>
        /// <param name="logfile">Optional - the log path (for future use, we can write the build's logs).</param>
        /// <returns>True if the build succeeded, else False.</returns>
        public bool BuildSolution(string solutionPath, string logfile = "somelogfile")
        {
            try
            {
                // Instantiate a new FileLogger to generate build log
                var logger = new FileLogger();
                logger.Parameters = @"logfile=" + TestsResourcesHelper.BuildLogPath;
                var projectCollection = new ProjectCollection();
                var globalProperty = new Dictionary<string, string>
                {
                    {"Configuration", "Debug"},
                    {"Platform", "Any CPU"},
                    {"OutputPath", TestsResourcesHelper.BuildPath},
                    {"nodereuse", "false"}
                };

                var buildRequest = new BuildRequestData(solutionPath, globalProperty, null, new [] { "Build" }, null);

                //register file logger using BuildParameters
                var bp = new BuildParameters(projectCollection);
                bp.Loggers = new List<Microsoft.Build.Framework.ILogger> { logger }.AsEnumerable();

                //build solution
                var buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);

                //Unregister all loggers to close the log file               
                projectCollection.UnregisterAllLoggers();
                projectCollection.Dispose();
                
                return buildResult.OverallResult == BuildResultCode.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
