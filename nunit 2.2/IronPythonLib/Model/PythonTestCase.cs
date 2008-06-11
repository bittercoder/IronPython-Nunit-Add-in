namespace IronPythonLib.Model
{
    /// <summary>
    /// Represents a single test case in our model
    /// </summary>
    public class PythonTestCase
    {
        private string _name;
        private Proc _testCase;

        public PythonTestCase(string name, Proc testCase)
        {
            _name = name;
            _testCase = testCase;
        }

        public void Execute()
        {
            _testCase();
        }

        public string Name
        {
            get { return _name; }
        }
    }
}