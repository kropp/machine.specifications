using System;

namespace Machine.Specifications.Framework
{
  public class Establish : ICallableFrameworkType
  {
    readonly Action _establish;

    public Establish(object establishDelegate)
    {
      var invoker = establishDelegate.GetType().GetMethod("Invoke", Framework.BindingFlags);
      _establish = () => invoker.Invoke(establishDelegate, Framework.NoArgs);
    }

    public string FullName
    {
      get { return "Machine.Specifications.Establish"; }
    }

    public Action GetInvoker()
    {
      return _establish;
    }
  }
}