using System;

namespace Machine.Specifications.Framework
{
  public class It : ICallableFrameworkType
  {
    readonly Action _it;

    public It(object itDelegate)
    {
      var invoker = itDelegate.GetType().GetMethod("Invoke", Framework.BindingFlags);
      _it = () => invoker.Invoke(itDelegate, Framework.NoArgs);
    }

    public string FullName
    {
      get { return "Machine.Specifications.It"; }
    }

    public Action GetInvoker()
    {
      return _it;
    }
  }
}