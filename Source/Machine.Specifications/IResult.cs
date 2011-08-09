using System.Collections.Generic;

namespace Machine.Specifications
{
  public interface IResult
  {
    IDictionary<string, IDictionary<string, string>> Supplements { get; }
    bool Passed { get; }
    bool Failing { get; }
    bool Ignored { get; }
    bool NotImplemented { get; }
    ExceptionResult Exception { get; }
    string ConsoleOut { get; }
    string ConsoleError { get; }
    bool HasSupplement(string name);
    IDictionary<string, string> GetSupplement(string name);
  }
}