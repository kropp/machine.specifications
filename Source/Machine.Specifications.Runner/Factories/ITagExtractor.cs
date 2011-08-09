using System;
using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Framework;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
  public interface ITagExtractor
  {
    IEnumerable<Tag> ExtractTags(Type type);
  }

  public class HybridTagExtractor : ITagExtractor
  {
    public IEnumerable<Tag> ExtractTags(Type type)
    {
      var interfaceTagExtractor = new InterfaceTagExtractor();
      var attributeTagExtractor = new AttributeTagExtractor();

      var tags = interfaceTagExtractor.ExtractTags(type).Union(attributeTagExtractor.ExtractTags(type)).Distinct();

      return tags.ToList();
    }
  }

  public class InterfaceTagExtractor : ITagExtractor
  {
    public IEnumerable<Tag> ExtractTags(Type type)
    {
      var tags = type.GetInterfaces().Where(x => x.FullName.StartsWith("Machine.Specifications.Tags`"))
        .SelectMany(x => x.GetGenericArguments())
        .Select(x => new Tag(x.Name)).Distinct();

      return tags.ToList();
    }
  }

  public class AttributeTagExtractor : ITagExtractor
  {
    public IEnumerable<Tag> ExtractTags(Type type)
    {
      var tags = type
        .GetAttributes(x => new TagsAttribute(x))
        .SelectMany(x => x.Tags)
        .Distinct()
        .Select(x => new Tag(x));

      return tags.ToList();
    }
  }
}
