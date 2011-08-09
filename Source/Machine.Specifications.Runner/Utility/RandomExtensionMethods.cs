using System;
using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Framework;

namespace Machine.Specifications.Utility
{
  public static class RandomExtensionMethods
  {
    internal static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var t in enumerable)
      {
        action(t);
      }
    }

    internal static void InvokeAll(this IEnumerable<Establish> contextActions)
    {
      contextActions.AllNonNull().Select(x => x.GetInvoker()).InvokeAll();
    }

    internal static void InvokeAll(this IEnumerable<Because> becauseActions)
    {
      becauseActions.AllNonNull().Select(x => x.GetInvoker()).InvokeAll();
    }

    internal static void InvokeAll(this IEnumerable<Cleanup> contextActions)
    {
      contextActions.AllNonNull().Select(x => x.GetInvoker()).InvokeAll();
    }

    static IEnumerable<T> AllNonNull<T>(this IEnumerable<T> elements) where T : class
    {
      return elements.Where(x => x != null);
    }

    static void InvokeAll(this IEnumerable<Action> actions)
    {
      actions.Each(x => x());
    }
  }
}