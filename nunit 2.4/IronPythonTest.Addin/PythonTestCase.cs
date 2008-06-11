namespace IronPythonTest.Addin
{
    /// <summary>
    /// Represents a single test case in our model
    /// </summary>
    public class PythonTestCase
    {
        private readonly string _name;
        private readonly Proc _testCase;

        public PythonTestCase(string name, Proc testCase)
        {
            _name = name;
            _testCase = testCase;
        }

        public string Name
        {
            get { return _name; }
        }

        public void Execute()
        {
            _testCase();
        }
    }
}