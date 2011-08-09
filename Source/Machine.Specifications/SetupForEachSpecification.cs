using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class SetupForEachSpecificationAttribute : Attribute
  {
  }
}
