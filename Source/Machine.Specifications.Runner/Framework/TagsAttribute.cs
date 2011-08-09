using System.Collections.Generic;

namespace Machine.Specifications.Framework
{
  class TagsAttribute : IFrameworkType
  {
    readonly IEnumerable<string> _tags;

    public TagsAttribute(object tagsAttribute)
    {
      var tags = tagsAttribute.GetType().GetProperty("Tags", Framework.BindingFlags);
      _tags = (IEnumerable<string>) tags.GetGetMethod().Invoke(tagsAttribute, Framework.NoArgs);
    }

    public IEnumerable<string> Tags
    {
      get { return _tags; }
    }

    public string FullName
    {
      get { return "Machine.Specifications.TagsAttribute"; }
    }
  }
}