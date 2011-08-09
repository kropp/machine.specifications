using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Framework;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
  public class ContextFactory
  {
    readonly BehaviorFactory _behaviorFactory;
    readonly SpecificationFactory _specificationFactory;
    static int _allowedNumberOfBecauseBlocks = 1;

    public ContextFactory()
    {
      _specificationFactory = new SpecificationFactory();
      _behaviorFactory = new BehaviorFactory();
    }

    public Context CreateContextFrom(object instance, FieldInfo fieldInfo)
    {
      if (fieldInfo.FieldType == typeof(It))
      {
        return CreateContextFrom(instance, new[] {fieldInfo});
      }
      return CreateContextFrom(instance);
    }

    public Context CreateContextFrom(object instance)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFieldsOfType<It>()
        .Union(type.GetPrivateFieldsOfType<Behaves_like>());

      return CreateContextFrom(instance, fieldInfos);
    }

    Context CreateContextFrom(object instance, IEnumerable<FieldInfo> acceptedSpecificationFields)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();
      var itFieldInfos = new List<FieldInfo>();
      var itShouldBehaveLikeFieldInfos = new List<FieldInfo>();

      var contextClauses = ExtractPrivateFieldValues<Establish>(instance, true);
      contextClauses.Reverse();

      var cleanupClauses = ExtractPrivateFieldValues<Cleanup>(instance, true);

      var becauses = ExtractPrivateFieldValues<Because>(instance, false);
      becauses.Reverse();

      if (becauses.Count > _allowedNumberOfBecauseBlocks)
      {
        throw new SpecificationUsageException("There can only be one Because clause.");
      }

      var concern = ExtractSubject(type);
      var isSetupForEachSpec = IsSetupForEachSpec(type);

      var isIgnored = type.HasAttribute<IgnoreAttribute>();
      var tags = ExtractTags(type);
      var context = new Context(type,
                                instance,
                                contextClauses,
                                becauses,
                                cleanupClauses,
                                concern,
                                isIgnored,
                                tags,
                                isSetupForEachSpec);

      foreach (var info in fieldInfos)
      {
        if (acceptedSpecificationFields.Contains(info) &&
            info.FieldType.CanBeCastTo<It>())
        {
          itFieldInfos.Add(info);
        }

        if (acceptedSpecificationFields.Contains(info) &&
            info.FieldType.CanBeCastTo<Behaves_like>())
        {
          itShouldBehaveLikeFieldInfos.Add(info);
        }
      }

      CreateSpecifications(itFieldInfos, context);
      CreateSpecificationsFromBehaviors(itShouldBehaveLikeFieldInfos, context);

      return context;
    }

    static IEnumerable<Tag> ExtractTags(Type type)
    {
      var extractor = new AttributeTagExtractor();
      return extractor.ExtractTags(type);
    }

    static bool IsSetupForEachSpec(Type type)
    {
      return type.HasAttribute<SetupForEachSpecificationAttribute>();
    }

    static Subject ExtractSubject(Type type)
    {
      var attributes = type.GetAttributes(x => new SubjectAttribute(x));

      if (attributes.Count() > 1)
      {
        throw new SpecificationUsageException("Cannot have more than one Subject on a Context");
      }

      if (!attributes.Any())
      {
        return ExtractSubject(type.DeclaringType);
      }

      var attribute = attributes.First();

      return new Subject(attribute.SubjectType, attribute.Subject);
    }

    void CreateSpecifications(IEnumerable<FieldInfo> itFieldInfos, Context context)
    {
      foreach (var itFieldInfo in itFieldInfos)
      {
        var specification = _specificationFactory.CreateSpecification(context, itFieldInfo);
        context.AddSpecification(specification);
      }
    }

    void CreateSpecificationsFromBehaviors(IEnumerable<FieldInfo> itShouldBehaveLikeFieldInfos,
                                           Context context)
    {
      foreach (var itShouldBehaveLikeFieldInfo in itShouldBehaveLikeFieldInfos)
      {
        var behavior = _behaviorFactory.CreateBehaviorFrom(itShouldBehaveLikeFieldInfo, context);

        foreach (var specification in behavior.Specifications)
        {
          context.AddSpecification(specification);
        }
      }
    }

    static void CollectDetailsOf<T>(Type target, Func<object> instanceResolver, ICollection<T> items, bool ensureMaximumOfOne)
    {
      if (target == typeof(object) || target == null)
      {
        return;
      }
      var instance = instanceResolver();
      if (instance == null)
      {
        return;
      }

      var fields = target.GetPrivateFieldsOfType(typeof(T));

      if (ensureMaximumOfOne && fields.Count() > 1)
      {
        throw new SpecificationUsageException(String.Format("You cannot have more than one {0} clause in {1}",
                                                            typeof(T).Name,
                                                            target.FullName));
      }
      var field = fields.FirstOrDefault();

      if (field != null)
      {
        var val = (T) field.GetValue(instance);
        items.Add(val);
      }

      CollectDetailsOf(target.BaseType, () => instance, items, ensureMaximumOfOne);
      CollectDetailsOf(target.DeclaringType, () => Activator.CreateInstance(target.DeclaringType), items, ensureMaximumOfOne);
    }

    static List<T> ExtractPrivateFieldValues<T>(object instance, bool ensureMaximumOfOne) where T : ICallableFrameworkType
    {
      var delegates = new List<T>();
      var type = instance.GetType();
      CollectDetailsOf(type, () => instance, delegates, ensureMaximumOfOne);

      return delegates;
    }

    public static void ChangeAllowedNumberOfBecauseBlocksTo(int newValue)
    {
      _allowedNumberOfBecauseBlocks = newValue;
    }
  }
}