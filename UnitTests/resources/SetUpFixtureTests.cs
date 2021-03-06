using NUnit.Framework;

namespace UnitTests
{
  [SetUpFixture]
  public class SetUpFixtureWithSetupAndTeardown
  {
    [SetUp]
    public void Setup ()
    {
    }

    [TearDown]
    public void TearDown ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithSetupAndTeardownTransformed
  {
    [OneTimeSetUp]
    public void Setup ()
    {
    }

    [OneTimeTearDown]
    public void TearDown ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithSetupOnly
  {
    [SetUp]
    public void Setup ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithSetupOnlyTransformed
  {
    [OneTimeSetUp]
    public void Setup ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithTeardownOnly
  {
    [TearDown]
    public void TearDown ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithTeardownOnlyTransformed
  {
    [OneTimeTearDown]
    public void TearDown ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithOtherMethods
  {
    private void c ()
    {
    }

    [SetUp]
    public void Setup ()
    {
    }

    private void b ()
    {
    }

    [TearDown]
    public void TearDown ()
    {
    }

    private void a ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixtureWithOtherMethodsTransformed
  {
    private void c ()
    {
    }

    [OneTimeSetUp]
    public void Setup ()
    {
    }

    private void b ()
    {
    }

    [OneTimeTearDown]
    public void TearDown ()
    {
    }

    private void a ()
    {
    }
  }

  [SetUpFixture]
  public class SetUpFixture
  {
  }

  public class SetUpFixtureByInheritance : SetUpFixture
  {
    [SetUp]
    public void SetUp ()
    {
    }

    [TearDown]
    public void TearDown ()
    {
    }
  }
  
  public class SetUpFixtureByInheritanceTransformed : SetUpFixture
  {
    [OneTimeSetUp]
    public void SetUp ()
    {
    }

    [OneTimeTearDown]
    public void TearDown ()
    {
    }
  }
  
}