using System;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class SpecificationInfo
  {
    public string Name { get; set; }
    public string ContainingType { get; set; }
    public string FieldName { get; set; }

    public SpecificationInfo(string name, string containingType, string fieldName)
    {
      Name = name;
      ContainingType = containingType;
      FieldName = fieldName;
    }
  }
}
