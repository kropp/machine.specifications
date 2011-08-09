using System;

namespace Machine.Specifications.Framework
{
  public class ICleanupAfterEveryContextInAssembly
  {
    readonly Action _afterContextCleanup;

    public ICleanupAfterEveryContextInAssembly(object instance)
    {
      var afterContextCleanup = instance.GetType().GetMethod("AfterContextCleanup", Framework.BindingFlags);
      _afterContextCleanup = () => afterContextCleanup.Invoke(instance, new object[] {});
    }

    public static string FullName
    {
      get { return "Machine.Specifications.ICleanupAfterEveryContextInAssembly"; }
    }

    public void AfterContextCleanup()
    {
      _afterContextCleanup();
    }
  }
}