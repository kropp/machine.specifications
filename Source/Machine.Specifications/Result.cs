using System;

namespace Machine.Specifications
{
  [Serializable]
  public class ExceptionResult
  {
    readonly string _toString;

    public string FullTypeName { get; private set; }
    public string TypeName { get; private set; }
    public string Message { get; private set; }
    public string StackTrace { get; private set; }
    public ExceptionResult InnerExceptionResult { get; private set; }

    public ExceptionResult(Exception exception)
    {
      FullTypeName = exception.GetType().FullName;
      TypeName = exception.GetType().Name;
      Message = exception.Message;
      StackTrace = FilterStackTrace(exception.StackTrace);

      if (exception.InnerException != null)
      {
        InnerExceptionResult = new ExceptionResult(exception.InnerException);
      }

      _toString = exception.ToString();
    }

    public override string ToString()
    {
      return _toString;
    }

    #region Borrowed from XUnit to clean up the stack trace, licened under MS-PL

#if CLEAN_EXCEPTION_STACK_TRACE
    /// <summary>
    ///  A description of the regular expression:
    ///
    ///  \w+\sMachine\.Specifications
    ///      Alphanumeric, one or more repetitions
    ///      Whitespace
    ///      Machine
    ///      Literal .
    ///      Specifications
    ///      Literal .
    /// </summary>
    static Regex FrameworkStackLine = new Regex("\\w+\\sMachine\\.Specifications\\.",
      RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Filters the stack trace to remove all lines that occur within the testing framework.
    /// </summary>
    /// <param name="stackTrace">The original stack trace</param>
    /// <returns>The filtered stack trace</returns>
    static string FilterStackTrace(string stackTrace)
    {
      if (stackTrace == null)
        return null;      

      List<string> results = new List<string>();

      foreach (string line in SplitLines(stackTrace))
      {
        string trimmedLine = line.TrimStart();
        if (!IsFrameworkStackFrame(trimmedLine))
          results.Add(line);
      }

      return string.Join(Environment.NewLine, results.ToArray());
    }

    static bool IsFrameworkStackFrame(string trimmedLine)
    {
      // Anything in the Machine.Specifications namespace
      return FrameworkStackLine.IsMatch(trimmedLine);
    }

    // Our own custom String.Split because Silverlight/CoreCLR doesn't support the version we were using
    static IEnumerable<string> SplitLines(string input)
    {
      while (true)
      {
        int index = input.IndexOf(Environment.NewLine);

        if (index < 0)
        {
          yield return input;
          break;
        }

        yield return input.Substring(0, index);
        input = input.Substring(index + Environment.NewLine.Length);
      }
    }
#else
    // Do not change the line at all if you are not going to clean it
    static string FilterStackTrace(string stackTrace)
    {
      return stackTrace;
    }
#endif

    #endregion
  }
}
