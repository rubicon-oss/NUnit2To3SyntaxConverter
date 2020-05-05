# NUnit2To3SyntaxConverter

This is a tool designed for easing the effort of migrating large NUnit2 unit test suites
to NUnit3. Unfortunately, fully automated migration is basically impossible, so manual work is still required.

### Features

`NUnit2To3SyntaxConverter` supports the conversion of:
* `ExpectedExceptionAttribute`  
    will transform the last statement of the test to an `Assert.That(..., Throws...)` assertion
* various renamed assertion methods (see [here](https://github.com/NUnit/docs/wiki/Breaking-Changes#assertions-and-constraints) for a full list)
* `TestFixtureSetUp` and `TestFixtureTearDown` renaming
* SetupFixture `SetUp` and `TearDown` renaming

### Usage

`NUnit2To3SyntaxConverter` is a simple terminal application and supports the following arguments:

```
NUnit2To3SyntaxConverter 1.0.0
Copyright (C) 2020 NUnit2To3SyntaxConverter

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
Sample invocation:  
`NUnit2To3SyntaxConverter.exe C:\path\to\My\AwesomeSolution.sln --msbuildversion=16.4.29728.190 --features=ExpectedException,AssertRenaming`

### Recommended Migration Process

#### Step 1: Upgrade to NUnit 2.7.1
 Manually upgrade your NUnit dependencies to 2.7.1. This is release supports most of the new NUnit 3 syntax, but does not require any major changes to your tests.
 *It is also recommended to disable obsolete warnings during the migration process*

#### Step 2: Apply a conversion
 Run NUnit2To3SyntaxConverter over your codebase for a single conversion feature. e.g. `AssertRenaming`. The program will report warnings if it encounters tests, which have to be fixed manually.  
 Notable examples for warnings are for example the last statement in a test with an `ExpectedExceptionAttribute` is not convertible to an expression or is it is an NUnit Assertion itself.

#### Step 3: Manual fixes
 After the automated conversion you should be able to run your testsuite and fix any failing tests. This usually only happens with the
 `ExpectedException` transfromation as it assumes that the last statement in your tests is always the one that throws.

#### Step 4: Repeat Step 2 and 3 for all conversions
 After completing all conversions, you should have a testsuite compatible with NUnit 3. Make sure all of your tests are run and nothing fails.
 You should now be able to reenable the obsolete warnings.

#### Step 5: Update to latest NUnit 3
 Update your NUnit dependencies to the latest version (currently tested with 3.12). This upgrade can introduce compile errors as lots of the NUnit api was rewritten.
 For an official list of changes see [here](https://github.com/NUnit/docs/wiki/Breaking-Changes)

#### Step 6: Making your test run again
 One major breaking change introduced with NUnit 3 is the way the test environment handles the filesystem.
 In NUnit2 all test are run with the Environment.CurrentDirectory set to output directory of your testing assemblies.
 Nunit 3 changes this behavior and generates a temporary directory for every test run. For this reason every test that depends on this behavior,
 e.g. referencing files with relative paths, will break and have to be manually fixed.
 Nunit3 exposes the old working directory with the `TestContext.CurrentContext.TestDirectory` property. This should be used to reference files with absolute paths or
 set `Environment.CurrentDirectory` to emulate NUnit 2 behavior

### Other known pitfalls

If an unexpected exception occurs during execution of a `[OneTimeTeardown]` method, the TestFixture (and the entire test run) is reported as failed, 
but the individual tests that ran inside the TestFixture are **not** marked as failed. 
This appears to only happen with `[OneTimeTeardown]`, errors inside `[SetUp]`, `[Teardown]` and `[OneTimeSetUp]` methods **do** mark the individual tests as failed as well. 
Some IDEs like JetBrains Rider mark the TestFixture itself as failed if such an error occurs, but other IDEs or build systems may not. 
Specifically, TeamCity appears to ignore such failures and report the test run as succeeded. 
The NUnit3 console does report such errors, meaning they will be visible in the build log, but since the build may not be displayed as failed, they may not be prominent enough.