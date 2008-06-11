using NUnit.Framework;

namespace IronPythonLib.Tests.Model
{
    [TestFixture]
    public class DynamicPythonSuiteFixture
    {
        [Test]
        public void Create()
        {
            DynamicPythonSuite suite = new DynamicPythonSuite();
        }

        [Test]
        public void Setup()
        {
            DynamicPythonSuite suite = new DynamicPythonSuite();
            suite.Setup();

            Assert.AreEqual(1, suite.Scripts.Length);
            Assert.AreEqual("IronPythonLib.Tests.DynamicFixture.py", suite.Scripts[0]);
        }
    }
}