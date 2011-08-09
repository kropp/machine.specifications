using System;

using Machine.Specifications.Annotations;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  public class SubjectAttribute : Attribute
  {
    readonly string _subject;
    readonly Type _subjectType;

    public SubjectAttribute(Type subjectType)
    {
      _subjectType = subjectType;
    }

    public SubjectAttribute(Type subjectType, string subject)
    {
      _subjectType = subjectType;
      _subject = subject;
    }

    public SubjectAttribute(string subject)
    {
      _subject = subject;
    }

    public Type SubjectType
    {
      get { return _subjectType; }
    }

    public string Subject
    {
      get { return _subject; }
    }
  }
}