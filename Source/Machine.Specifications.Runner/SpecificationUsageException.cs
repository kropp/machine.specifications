using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
  [Serializable]
  public class SpecificationUsageException : Exception
  {
    public SpecificationUsageException()
    {
    }

    public SpecificationUsageException(string message) : base(message)
    {
    }

    public SpecificationUsageException(string message, Exception inner) : base(message, inner)
    {
    }

    protected SpecificationUsageException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}