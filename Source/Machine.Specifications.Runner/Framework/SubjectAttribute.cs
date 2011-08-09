using System;

namespace Machine.Specifications.Framework
{
  class SubjectAttribute : IFrameworkType
  {
    readonly string _subject;
    readonly Type _subjectType;

    public SubjectAttribute(object subjectAttribute)
    {
      var subjectType = subjectAttribute.GetType().GetProperty("SubjectType", Framework.BindingFlags);
      _subjectType = (Type) subjectType.GetGetMethod().Invoke(subjectAttribute, Framework.NoArgs);

      var subject = subjectAttribute.GetType().GetProperty("Subject", Framework.BindingFlags);
      _subject = (string) subject.GetGetMethod().Invoke(subjectAttribute, Framework.NoArgs);
    }

    public Type SubjectType
    {
      get { return _subjectType; }
    }

    public string Subject
    {
      get { return _subject; }
    }

    public string FullName
    {
      get { return "Machine.Specifications.SubjectAttribute"; }
    }
  }
}