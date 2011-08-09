using System;

namespace Machine.Specifications.Framework
{
  public class Because : ICallableFrameworkType
  {
    readonly Action _because;

    public Because(object becauseDelegate)
    {
      var invoker = becauseDelegate.GetType().GetMethod("Invoke", Framework.BindingFlags);
      _because = () => invoker.Invoke(becauseDelegate, Framework.NoArgs);
    }

    public string FullName
    {
      get { return "Machine.Specifications.Because"; }
    }

    public Action GetInvoker()
    {
      return _because;
    }
  }
}