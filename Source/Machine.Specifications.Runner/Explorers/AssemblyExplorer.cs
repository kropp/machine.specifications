using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Factories;
using Machine.Specifications.Framework;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    readonly ContextFactory contextFactory;

    public AssemblyExplorer()
    {
      contextFactory = new ContextFactory();
    }

    public IEnumerable<Context> FindContextsIn(Assembly assembly)
    {
      return EnumerateContextsIn(assembly).Select(x => CreateContextFrom(x));
    }

    public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace)
    {
      return EnumerateContextsIn(assembly)
        .Where(x => x.Namespace == targetNamespace)
        .Select(x => CreateContextFrom(x));
    }

    public IEnumerable<ICleanupAfterEveryContextInAssembly> FindAssemblyWideContextCleanupsIn(Assembly assembly)
    {
      return assembly
        .GetExportedTypes()
        .Where(x => x.GetInterfaces().Any(i => i.FullName == ICleanupAfterEveryContextInAssembly.FullName))
        .Select(x => new ICleanupAfterEveryContextInAssembly(Activator.CreateInstance(x)));
    }

    public IEnumerable<ISupplementSpecificationResults> FindSpecificationSupplementsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes()
        .Where(x =>
               x.GetInterfaces().Contains(typeof(ISupplementSpecificationResults)))
        .Select(x => (ISupplementSpecificationResults) Activator.CreateInstance(x));
    }

    public IEnumerable<IAssemblyContext> FindAssemblyContextsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes()
        .Where(x => x.GetInterfaces().Any(i => i.FullName == IAssemblyContext.FullName))
        .Select(x => new IAssemblyContext(Activator.CreateInstance(x)));
    }

    Context CreateContextFrom(Type type)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance);
    }

    Context CreateContextFrom(Type type, FieldInfo fieldInfo)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance, fieldInfo);
    }

    static bool IsContext(Type type)
    {
      return HasSpecificationMembers(type) && !type.HasAttribute<BehaviorsAttribute>();
    }

    static bool HasSpecificationMembers(Type type)
    {
      return !type.IsAbstract &&
             (type.GetPrivateFieldsOfType<It>().Any() ||
              type.GetPrivateFieldsOfType<Behaves_like>().Any());
    }

    static IEnumerable<Type> EnumerateContextsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes().Where(IsContext)
        .OrderBy(t => t.Namespace);
    }

    public Context FindContexts(Type type)
    {
      if (IsContext(type))
      {
        return CreateContextFrom(type);
      }

      return null;
    }

    public Context FindContexts(FieldInfo info)
    {
      Type type = info.ReflectedType;
      if (IsContext(type))
      {
        return CreateContextFrom(type, info);
      }

      return null;
    }
  }
}
