using System;
using System.Collections.Generic;

namespace Machine.Specifications.Utility
{
  public static class RandomExtensionMethods
  {
    public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var t in enumerable)
      {
        action(t);
      }
    }
  }
}