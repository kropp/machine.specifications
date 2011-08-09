using System;
using System.Collections.Generic;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class TagsAttribute : Attribute
  {
    readonly List<string> _tags;

    public TagsAttribute(string tag, params string[] additionalTags)
    {
      _tags = new List<string> {tag};
      _tags.AddRange(additionalTags);
    }

    public IEnumerable<string> Tags
    {
      get { return _tags; }
    }
  }
}