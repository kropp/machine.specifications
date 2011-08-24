using System.Collections.Generic;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class SubjectFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ContextCache _cache;
    readonly IProject _project;
    string _assemblyPath;

    public SubjectFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
      _assemblyPath = assemblyPath;
    }

    public SubjectElement CreateSubject(ITypeElement type)
    {
      if (_cache.Classes.ContainsKey(type))
      {
        return _cache.Subjects[type];
      }

      var subject = GetOrCreateSubjectElement(_provider,
                                              _project,
                                              _projectEnvoy,
                                              type.GetSubjectString());

#if RESHARPER_6
      foreach (var child in subject.Children)
      {
        child.State = UnitTestElementState.Pending;
      }
#endif

      _cache.Subjects.Add(type, subject);
      return subject;
    }

    public SubjectElement CreateSubject(IMetadataTypeInfo type)
    {
      return GetOrCreateSubjectElement(_provider,
                                       _project,
                                       _projectEnvoy,
                                       type.GetSubjectString() ?? "Specifications");
    }

    public static SubjectElement GetOrCreateSubjectElement(MSpecUnitTestProvider provider,
                                                           IProject project,
                                                           ProjectModelElementEnvoy projectEnvoy,
                                                           string subject)
    {
#if RESHARPER_6
      var id = SubjectElement.CreateId(subject);
      var subjectElement = provider.UnitTestManager.GetElementById(project, id) as SubjectElement;
      if (subjectElement != null)
      {
        subjectElement.State = UnitTestElementState.Valid;
        return subjectElement;
      }
#endif

      return new SubjectElement(provider,
                                projectEnvoy,
                                subject);
    }

    public void UpdateChildState(ITypeElement type)
    {
      SubjectElement element;
      if (!_cache.Subjects.TryGetValue(type, out element))
      {
        return;
      }

      foreach (var unitTestElement in element.Children.Where(x => x.State == UnitTestElementState.Pending))
      {
        unitTestElement.State = UnitTestElementState.Invalid;
      }
    }
  }
}