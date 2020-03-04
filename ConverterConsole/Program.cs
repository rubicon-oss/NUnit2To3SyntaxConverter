#region copyright

// 
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

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit2To3SyntaxConverter.ExpectedException;
using NUnit2To3SyntaxConverter.RenamedAsserts;

namespace NUnit2To3SyntaxConverter.ConverterConsole
{
  internal class Program
  {
    private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
    {
      public void Report (ProjectLoadProgress loadProgress)
      {
        var projectDisplay = Path.GetFileName (loadProgress.FilePath);
        if (loadProgress.TargetFramework != null)
          projectDisplay += $" ({loadProgress.TargetFramework})";

        Console.WriteLine ($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
      }
    }

    public static async Task Main (string[] args)
    {
      await Parser.Default.ParseArguments<CmdLineOptions> (args)
          .MapResult (
              RunConverter,
              _ => Task.FromResult (1));
    }

    private static async Task<int> RunConverter (CmdLineOptions options)
    {
      var msBuild = LocateMsBuild (options);
      MSBuildLocator.RegisterInstance (msBuild);
      using var workspace = MSBuildWorkspace.Create();
      workspace.WorkspaceFailed += (sender, args) => { Console.WriteLine (args.Diagnostic); };

      var solution = await workspace.OpenSolutionAsync (options.SolutionPath, new ConsoleProgressReporter());
      var converters = options.FeatureFlags.Select (CreateConverter);

      await new NUnitMigration (
              MigrationOptions.DefaultOptions
                  .WithConverters (converters))
          .Migrate (solution);

      return 0;
    }


    private static IDocumentConverter CreateConverter (FeatureFlags flag)
    {
      return flag switch
      {
          FeatureFlags.ExpectedException => new ExpectedExceptionDocumentConverter(),
          FeatureFlags.AssertRenaming => new AssertRenamingDocumentConverter(),
          _ => throw new InvalidOperationException ("Exhaustive switch is not exhaustive.")
      };
    }

    private static VisualStudioInstance? LocateMsBuild (CmdLineOptions options)
    {
      var queriedInstances = QueryVisualStudioInstances (options).ToList();

      if (queriedInstances.Count == 0)
      {
        DisplayNoMsBuildFoundError();
        return null;
      }

      if (queriedInstances.Count > 1)
      {
        DisplayMoreThanOneMsBuildFoundError (queriedInstances);
        return null;
      }

      return queriedInstances.Single();
    }

    private static IEnumerable<VisualStudioInstance> QueryVisualStudioInstances (CmdLineOptions options)
    {
      if (!string.IsNullOrWhiteSpace (options.MsBuildPath))
        MSBuildLocator.RegisterMSBuildPath (options.MsBuildPath);

      var instances = MSBuildLocator.QueryVisualStudioInstances();

      if (Version.TryParse (options.MsBuildVersion, out var version))
        instances = instances.Where (vs => vs.Version.Equals (version));

      if (options.MsBuildPath != null && File.Exists (options.MsBuildPath))
        instances = instances.Where (vs => Path.GetFullPath (vs.MSBuildPath) == Path.GetFullPath (options.MsBuildPath));
      return instances;
    }

    private static void DisplayMoreThanOneMsBuildFoundError (List<VisualStudioInstance> queriedInstances)
    {
      Console.Error.WriteLine ("Ambiguous MsBuild configuration supplied!");
      Console.Error.WriteLine ("Multiple MsBuild version matched your supplied arguments: ");
      for (var i = 0; i < queriedInstances.Count; i++)
      {
        Console.Error.WriteLine ($"    Instance {i + 1}");
        Console.Error.WriteLine ($"        Name        : {queriedInstances[i].Name}");
        Console.Error.WriteLine ($"        Version     : {queriedInstances[i].Version}");
        Console.Error.WriteLine ($"        MsBuildPath : {queriedInstances[i].MSBuildPath}");
      }
    }

    private static void DisplayNoMsBuildFoundError ()
    {
      Console.Error.WriteLine ("No MsBuild instances found using supplied arguments!");
      Console.Error.WriteLine ("Choose one of the following: ");
      var allInstances = MSBuildLocator.QueryVisualStudioInstances().ToList();
      for (var i = 0; i < allInstances.Count; i++)
      {
        Console.Error.WriteLine ($"    Instance {i + 1}");
        Console.Error.WriteLine ($"        Name        : {allInstances[i].Name}");
        Console.Error.WriteLine ($"        Version     : {allInstances[i].Version}");
        Console.Error.WriteLine ($"        MsBuildPath : {allInstances[i].MSBuildPath}");
      }
    }
  }
}