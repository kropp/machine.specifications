using System;
using System.Collections.Generic;

namespace Machine.Specifications
{
  public enum Status
  {
    Failing,
    Passing,
    NotImplemented,
    Ignored
  }

  [Serializable]
  public class Result
  {
    readonly Status _status;
    readonly IDictionary<string, IDictionary<string, string>> _supplements = new Dictionary<string, IDictionary<string, string>>();

    public IDictionary<string, IDictionary<string, string>> Supplements
    {
      get { return _supplements; }
    }

    public bool Passed
    {
      get { return _status == Status.Passing; }
    }

    public ExceptionResult Exception { get; private set; }

    public Status Status
    {
      get { return _status; }
    }

    public string ConsoleOut
    {
      get;
      internal set;
    }

    public string ConsoleError
    {
      get;
      internal set;
    }

    public bool HasSupplement(string name)
    {
      return _supplements.ContainsKey(name);
    }

    public IDictionary<string, string> GetSupplement(string name)
    {
      return _supplements[name];
    }

    private Result(Exception exception)
    {
      _status = Status.Failing;
      Exception = new ExceptionResult(exception);
    }

    private Result(Status status)
    {
      _status = status;
    }

    private Result(Result result, string supplementName, IDictionary<string, string> supplement)
    {
      _status = result.Status;
      Exception = result.Exception;

      foreach (var pair in result._supplements)
      {
        _supplements.Add(pair);
      }

      if (HasSupplement(supplementName))
      {
        throw new ArgumentException("Result already has supplement named: " + supplementName, "supplementName");
      }

      _supplements.Add(supplementName, supplement);
      ConsoleOut = result.ConsoleOut;
      ConsoleError = result.ConsoleError;
    }

    public static Result Pass()
    {
      return new Result(Status.Passing);
    }

    public static Result Ignored()
    {
      return new Result(Status.Ignored);
    }

    public static Result NotImplemented()
    {
      return new Result(Status.NotImplemented);
    }

    public static Result Failure(Exception exception)
    {
      return new Result(exception);
    }

    public static Result ContextFailure(Exception exception)
    {
      return new Result(exception);
    }

    public static Result Supplement(Result result, string supplementName, IDictionary<string, string> supplement)
    {
      return new Result(result, supplementName, supplement);
    }
  }
}