using System.Collections.Generic;

using JetBrains.ReSharper.Psi;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextCache
  {
    public ContextCache()
    {
      Classes = new Dictionary<ITypeElement, ContextElement>();
      Subjects = new Dictionary<ITypeElement, SubjectElement>();
    }

    public IDictionary<ITypeElement, ContextElement> Classes
    {
      get;
      private set;
    }
    
    public IDictionary<ITypeElement, SubjectElement> Subjects
    {
      get;
      private set;
    }
  }
}
