// Copyright (c) rubicon IT GmbH
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using CommandLine;
using JetBrains.Annotations;

namespace NUnit2To3SyntaxConverter.ConverterConsole
{
  [UsedImplicitly]
  public class CmdLineOptions
  {
    public CmdLineOptions (string solutionPath, IEnumerable<FeatureFlags> featureFlags, string? msBuildVersion = null, string? msBuildPath = null)
    {
      SolutionPath = solutionPath;
      FeatureFlags = featureFlags;
      MsBuildVersion = msBuildVersion;
      MsBuildPath = msBuildPath;
    }

    [Value (0, Required = true, HelpText = "Path to a folder containing a solution file", MetaName = "solution")]
    public string SolutionPath { get; }

    [Option (
        "features",
        Required = true,
        HelpText = "List of conversion steps to apply: one or multiple of {ExpectedException, AssertRenaming, TestFixture, SetUpFixture}")]
    public IEnumerable<FeatureFlags> FeatureFlags { get; }

    [Option ("msbuildversion", Required = false, MetaValue = "VERSION")]
    public string? MsBuildVersion { get; }

    [Option ("msbuildpath", Required = false, MetaValue = "PATH")]
    public string? MsBuildPath { get; }
  }
}