namespace Machine.Specifications.Runner
{
  public interface ISpecificationResultProvider
  {
    bool FailureOccurred { get; }
  }
}