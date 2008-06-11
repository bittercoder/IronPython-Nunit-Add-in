using IronPythonLib.Model;

namespace IronPythonLib.Tests
{
    public class MyPythonSuite : AbstractPythonSuite
    {
        public MyPythonSuite()
            : base("MyPythonFixture.py")
        {
        }
    }
}