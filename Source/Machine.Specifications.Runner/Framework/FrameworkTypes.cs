using System;
using System.Reflection;

namespace Machine.Specifications.Framework
{
  static class Framework
  {
    public static BindingFlags BindingFlags
    {
      get { return BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod; }
    }

    public static object[] NoArgs
    {
      get { return new object[] {}; }
    }
  }

  public interface IFrameworkType
  {
    string FullName { get; }
  }

  interface ICallableFrameworkType : IFrameworkType
  {
    Action GetInvoker();
  }

  class Behaves_like : ICallableFrameworkType
  {
    public string FullName
    {
      get { return "Machine.Specifications.Behaves_like`1"; }
    }

    public Action GetInvoker()
    {
      throw new NotImplementedException();
    }
  }

  class IgnoreAttribute : IFrameworkType
  {
    public string FullName
    {
      get { return "Machine.Specifications.IgnoreAttribute"; }
    }
  }

  class SetupForEachSpecificationAttribute : IFrameworkType
  {
    public string FullName
    {
      get { return "Machine.Specifications.SetupForEachSpecificationAttribute"; }
    }
  }

  class BehaviorsAttribute : IFrameworkType
  {
    public string FullName
    {
      get { return "Machine.Specifications.BehaviorsAttribute"; }
    }
  }
}