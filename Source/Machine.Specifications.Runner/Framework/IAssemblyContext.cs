using System;

namespace Machine.Specifications.Framework
{
  public class IAssemblyContext
  {
    readonly Action _onAssemblyComplete;
    readonly Action _onAssemblyStart;

    public IAssemblyContext(object instance)
    {
      var onAssemblyStart = instance.GetType().GetMethod("OnAssemblyStart", Framework.BindingFlags);
      _onAssemblyStart = () => onAssemblyStart.Invoke(instance, new object[] {});

      var onAssemblyComplete = instance.GetType().GetMethod("OnAssemblyComplete", Framework.BindingFlags);
      _onAssemblyComplete = () => onAssemblyComplete.Invoke(instance, new object[] {});
    }

    public static string FullName
    {
      get { return "Machine.Specifications.IAssembyContext"; }
    }

    public void OnAssemblyStart()
    {
      _onAssemblyStart();
    }

    public void OnAssemblyComplete()
    {
      _onAssemblyComplete();
    }
  }
}