namespace IronPythonLib
{
    /// <summary>
    /// Used when looking at how a python class can implement an interface in .Net
    /// </summary>
    public interface IStringTransformer
    {
        string Transform(string input);
    }
}