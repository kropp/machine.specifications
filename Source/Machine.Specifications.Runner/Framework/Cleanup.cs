using System;

namespace Machine.Specifications.Framework
{
  public class Cleanup : ICallableFrameworkType
  {
    readonly Action _cleanup;

    public Cleanup(object cleanupDelegate)
    {
      var invoker = cleanupDelegate.GetType().GetMethod("Invoke", Framework.BindingFlags);
      _cleanup = () => invoker.Invoke(cleanupDelegate, Framework.NoArgs);
    }

    public string FullName
    {
      get { return "Machine.Specifications.Cleanup"; }
    }

    public Action GetInvoker()
    {
      return _cleanup;
    }
  }
}