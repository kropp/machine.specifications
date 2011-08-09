using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Framework;

namespace Machine.Specifications.Utility
{
  public static class ReflectionHelper
  {
    public static IEnumerable<FieldInfo> GetPrivateFields(this Type type)
    {
      return type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
    }

    public static IEnumerable<FieldInfo> GetStaticProtectedOrInheritedFields(this Type type)
    {
      return type
        .GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
        .Where(x => !x.IsPrivate);
    }

    public static IEnumerable<FieldInfo> GetPrivateFieldsOfType<T>(this Type type) where T : IFrameworkType
    {
      return type.GetPrivateFields().Where(x => x.FieldType.CanBeCastTo<T>());
    }

    public static IEnumerable<FieldInfo> GetPrivateFieldsOfType(this Type type, Type fieldType)
    {
      if (fieldType.CanBeCastTo<ICallableFrameworkType>())
      {
        // HACK This stinks
        return type.GetPrivateFields().Where(x => x.FieldType.CanBeCastTo((IFrameworkType) fieldType));
      }

      return type.GetPrivateFields().Where(x => x.FieldType.CanBeCastTo(fieldType));
    }

    public static FieldInfo GetStaticProtectedOrInheritedFieldNamed(this Type type, string fieldName)
    {
      return type.GetStaticProtectedOrInheritedFields().Where(x => x.Name == fieldName).SingleOrDefault();
    }

    internal static bool HasAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider) where TAttribute : IFrameworkType
    {
      return attributeProvider
        .GetCustomAttributes(typeof(object), true)
        .Any(x => x.GetType().CanBeCastTo<TAttribute>());
    }

    internal static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider attributeProvider,
                                                                      Func<object, TAttribute> createAdapter)
      where TAttribute : IFrameworkType
    {
      return attributeProvider
        .GetCustomAttributes(typeof(object), true)
        .Where(x => x.GetType().CanBeCastTo<TAttribute>())
        .Select(createAdapter);
    }

    internal static bool CanBeCastTo<T>(this Type type) where T : IFrameworkType
    {
      var frameworkType = (ICallableFrameworkType) Activator.CreateInstance(typeof(T));
      return type.FullName == frameworkType.FullName;
    }

    internal static bool CanBeCastTo(this Type type, IFrameworkType frameworkType)
    {
      if (type.IsGenericType)
      {
        return type.GetGenericTypeDefinition().Name == frameworkType.FullName;
      }

      return type.FullName == frameworkType.FullName;
    }

    internal static bool CanBeCastTo(this Type type, Type fieldType)
    {
      if (type.IsGenericType)
      {
        return type.GetGenericTypeDefinition() == fieldType;
      }
      return type == fieldType;
    }
  }
}