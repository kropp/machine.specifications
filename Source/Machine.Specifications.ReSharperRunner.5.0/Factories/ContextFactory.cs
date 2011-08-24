using System.Collections.Generic;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextFactory
  {
    readonly string _assemblyPath;

    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ContextCache _cache;
    readonly IProject _project;

    public ContextFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
      _assemblyPath = assemblyPath;
    }

    public ContextElement CreateContext(ITypeElement type)
    {
      if (_cache.Classes.ContainsKey(type))
      {
        return _cache.Classes[type];
      }

      var clazz = type as IClass;
      if (clazz == null)
      {
        return null;
      }

      SubjectElement subjectElement;
      _cache.Subjects.TryGetValue(clazz, out subjectElement);
      if (subjectElement == null)
      {
        return null;
      }

      var context = GetOrCreateContextElement(_provider,
                                              _project,
                                              _projectEnvoy,
                                              subjectElement,
#if RESHARPER_6
                                              type.GetClrName().FullName,
#else
                                              type.CLRName,
#endif
                                              _assemblyPath,
                                              type.GetTags(),
                                              type.IsIgnored());

#if RESHARPER_6
      foreach (var child in context.Children)
      {
        child.State = UnitTestElementState.Pending;
      }
#endif

      _cache.Classes.Add(type, context);
      return context;
    }

    public ContextElement CreateContext(SubjectElement subjectElement, IMetadataTypeInfo type)
    {
      return GetOrCreateContextElement(_provider,
                                       _project,
                                       _projectEnvoy,
                                       subjectElement,
                                       type.FullyQualifiedName,
                                       _assemblyPath,
                                       type.GetTags(),
                                       type.IsIgnored());
    }

    public static ContextElement GetOrCreateContextElement(MSpecUnitTestProvider provider,
                                                           IProject project,
                                                           ProjectModelElementEnvoy projectEnvoy,
                                                           SubjectElement subjectElement,
                                                           string typeName,
                                                           string assemblyLocation,
                                                           ICollection<string> tags,
                                                           bool isIgnored)
    {
#if RESHARPER_6
      var id = ContextElement.CreateId(subjectElement, typeName);
      var contextElement = provider.UnitTestManager.GetElementById(project, id) as ContextElement;
      if (contextElement != null)
      {
        contextElement.State = UnitTestElementState.Valid;
        return contextElement;
      }
#endif

      return new ContextElement(provider,
                                projectEnvoy,
                                subjectElement,
                                typeName,
                                assemblyLocation,
                                tags,
                                isIgnored);
    }

#if RESHARPER_6
    public void UpdateChildState(ITypeElement type)
    {
      ContextElement element;
      if (!_cache.Classes.TryGetValue(type, out element))
      {
        return;
      }

      foreach (var unitTestElement in element.Children.Where(x => x.State == UnitTestElementState.Pending))
      {
        unitTestElement.State = UnitTestElementState.Invalid;
      }
    }
#endif
  }
}