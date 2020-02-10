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
using Microsoft.CodeAnalysis;

namespace NUnit2To3SyntaxConverter
{
    public class NUnitAssemblyFilter
    {
        private const string c_nUnitAssemblyName = "nunit.framework";
        private readonly Version _maxSupportedNUnitVersion = new Version(2, 7, 0);
        private readonly Version _minSupportedNUnitVersion = new Version(2, 6, 0);

        public bool IsSupportedAssembly (AssemblyIdentity assemblyIdentity)
        {
            // we can only verify singed assemblies 
            if (!assemblyIdentity.IsStrongName)
                return false;
            // if the assembly is nunit between version 2.6 and 2.7 it is supported
            return assemblyIdentity.Name == c_nUnitAssemblyName
                   && IsInVersionRange (assemblyIdentity.Version, _minSupportedNUnitVersion, _maxSupportedNUnitVersion);
        }

        private bool IsInVersionRange (Version toCheck, Version min, Version max)
        {
            return min <= toCheck && toCheck <= max;
        }

    }
}