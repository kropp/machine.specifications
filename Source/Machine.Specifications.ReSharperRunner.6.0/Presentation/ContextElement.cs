using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.Factories;
using Machine.Specifications.Utility.Internal;

using ContextFactory = Machine.Specifications.ReSharperRunner.Factories.ContextFactory;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class ContextElement : Element, ISerializableElement
  {
    readonly string _assemblyLocation;
    readonly IEnumerable<UnitTestElementCategory> _categories;

    public ContextElement(MSpecUnitTestProvider provider,
                          ProjectModelElementEnvoy projectEnvoy,
                          SubjectElement subjectElement,
                          string typeName,
                          string assemblyLocation,
                          IEnumerable<string> tags,
                          bool isIgnored)
      : base(provider, subjectElement, projectEnvoy, typeName, isIgnored)
    {
      _assemblyLocation = assemblyLocation;

      if (tags != null)
      {
        _categories = UnitTestElementCategory.Create(tags);
      }
    }

    public override string ShortName
    {
      get { return Kind; }
    }

    public SubjectElement Subject
    {
      get { return (SubjectElement)Parent; }
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public override string GetPresentation()
    {
      return new ClrTypeName(GetTypeClrName()).ShortName.ToFormat();
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      return GetDeclaredType();
    }

    public override string Kind
    {
      get { return "Context"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return _categories; }
    }

    public override string Id
    {
      get { return CreateId(Subject, TypeName); }
    }

    public void WriteToXml(XmlElement parent)
    {
      parent.SetAttribute("typeName", TypeName);
      parent.GetAttribute("assemblyLocation", AssemblyLocation);
      parent.SetAttribute("isIgnored", Explicit.ToString());
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(provider.Solution, projectId) as IProject;
      if (project == null)
      {
        return null;
      }

      var subject = parentElement as SubjectElement;
      if (subject == null)
      {
        return null;
      }

      var typeName = parent.GetAttribute("typeName");
      var assemblyLocation = parent.GetAttribute("assemblyLocation");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));

      return ContextFactory.GetOrCreateContextElement(provider,
                                                      project,
                                                      ProjectModelElementEnvoy.Create(project),
                                                      subject,
                                                      typeName,
                                                      assemblyLocation,
                                                      EmptyArray<string>.Instance,
                                                      isIgnored);
    }

    public static string CreateId(SubjectElement subject, string typeName)
    {
      var id = String.Format("{0}.{1}", subject.Id, typeName);
      System.Diagnostics.Debug.WriteLine("CE  " + id);
      return id;
    }
  }
}