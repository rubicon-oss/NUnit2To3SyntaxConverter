# NUnit2To3SyntaxConverter

This is a tool designed for easing the effort of migrating large nunit2 unit test suites
to nunit3. Unfortunately, fully automated migration is basically impossible, so manual work is still required.

### Features

`NUnit2To3SyntaxConverter` supports the conversion of:
* ExpectedExceptionAttribute
* various renamed assertion methods (see [here](https://github.com/nunit/docs/wiki/Breaking-Changes#assertions-and-constraints) for a full list)
* `TestFixtureSetUp` and `TestFixtureTearDown` renaming
* SetupFixture `SetUp` and `TearDown` renaming

### Usage

`NUnit2To3SyntaxConverter` is a simple terminal application and supports the following arguments:

```
ConverterConsole 1.0.0
Copyright (C) 2020 ConverterConsole

  --features                  Required. List of conversion steps to apply: 
                              one or multiple of 
                              {ExpectedException, AssertRenaming, TestFixture, SetUpFixture}

  --msbuildversion=VERSION

  --msbuildpath=PATH

  --help                      Display this help screen.

  --version                   Display version information.

  solution (pos. 0)           Required. Path to a folder containing a solution
                              file
```
Sample invocation: `NUnit2To3SyntaxConverter.exe --msbuildversion=16.4.29728.190 --features=ExpectedException,AssertRenaming C:\path\to\My\AwesomeSolution.sln`

The program will automatically search for any projects using nunit2 syntax and apply the supplied conversions.