using System.Collections.Generic;

using Machine.Specifications.Factories;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using Machine.Testing;
using NUnit.Framework;

namespace Machine.Specifications.Model
{
  [TestFixture]
  public class SpecificationTests : With<ContextWithSingleSpecification>
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      Run(context);
    }

    [Test]
    public void ShouldEstablishContext()
    {
      ContextWithSingleSpecification.BecauseInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallBeforeEach()
    {
      ContextWithSingleSpecification.ContextInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCleanup()
    {
      ContextWithSingleSpecification.CleanupInvoked.ShouldBeTrue();
    }
  }

  public class With<T> : TestsFor<ContextFactory> where T : IFakeContext, new()
  {
    protected Context context;
    public override void BeforeEachTest()
    {
      IFakeContext fakeContext = new T();
      fakeContext.Reset();

      context = Target.CreateContextFrom(fakeContext);
    }

    public IEnumerable<Result> Run(Context context)
    {
      var runner = ContextRunnerFactory.GetContextRunnerFor(context);
      return runner.Run(context, new RunListenerBase(), RunOptions.Default, new Framework.ICleanupAfterEveryContextInAssembly[] {}, new ISupplementSpecificationResults[] {});
    }
  }
}
