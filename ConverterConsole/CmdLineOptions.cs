﻿﻿using System;
using CommandLine;

namespace NUnit2To3SyntaxConverter.ConverterConsole
{
    public class CmdLineOptions
    {
        public CmdLineOptions (string solutionPath, string msBuildVersion = null, string msBuildPath = null)
        {
            SolutionPath = solutionPath;
            MsBuildVersion = msBuildVersion;
            MsBuildPath = msBuildPath;
        }
        
        [Value(0, Required = true, HelpText = "Path to a folder containing a solution file", MetaName = "solution")]
        public string SolutionPath { get; }
        
        [Option("msbuildversion", Required = false, MetaValue = "VERSION")]
        public string MsBuildVersion { get; }

        [Option("msbuildpath", Required = false, MetaValue = "PATH")]
        public string MsBuildPath { get; }

    }
}