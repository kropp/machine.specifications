using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class SubjectElement : Element, ISerializableElement
  {
    readonly string _subject;

    public SubjectElement(MSpecUnitTestProvider provider,
                          ProjectModelElementEnvoy projectEnvoy,
                          string subject)
      : base(provider, null, projectEnvoy, null, false)
    {
      _subject = subject;
    }

    public override string ShortName
    {
      get { return Kind; }
    }

    public override string GetPresentation()
    {
      return GetSubject();
    }

    string GetSubject()
    {
      if (String.IsNullOrEmpty(_subject))
      {
        return "Specifications";
      }

      return _subject;
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      return null;
    }

    public override string Kind
    {
      get { return "Subject"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return Enumerable.Empty<UnitTestElementCategory>(); }
    }

    public override string Id
    {
      get { return CreateId(_subject); }
    }

    public void WriteToXml(XmlElement parent)
    {
      parent.GetAttribute("subject", GetSubject());
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(provider.Solution, projectId) as IProject;
      if (project == null)
      {
        return null;
      }

      var subject = parent.GetAttribute("subject");

      return SubjectFactory.GetOrCreateSubjectElement(provider,
                                                      project,
                                                      ProjectModelElementEnvoy.Create(project),
                                                      subject);
    }

    public static string CreateId(string subject)
    {
      var id = String.Format("{0}", subject);
      System.Diagnostics.Debug.WriteLine("SE  " + id);
      return id;
    }
  }
}