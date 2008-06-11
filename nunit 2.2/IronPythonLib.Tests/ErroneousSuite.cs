using IronPythonLib.Model;

namespace IronPythonLib.Tests
{
    public class ErroneousSuite : AbstractPythonSuite
    {
        public ErroneousSuite()
            : base("ErroneousFixtures.py")
        {
        }
    }
}