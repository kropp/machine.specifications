namespace Machine.Specifications
{
  public delegate void Establish();

  public delegate void Because();

  public delegate void It();
  public delegate void Behaves_like<TBehavior>();

  public delegate void Cleanup();
}