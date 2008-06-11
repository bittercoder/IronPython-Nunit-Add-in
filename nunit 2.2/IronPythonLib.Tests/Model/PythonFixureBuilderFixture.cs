using System.Collections.Generic;
using IronPythonLib.Tests;
using NUnit.Framework;

namespace IronPythonLib.Model.Tests
{
    [TestFixture]
    public class PythonFixureBuilderFixture
    {
        [Test]
        public void BuildFixtures()
        {
            MyPythonSuite suite = new MyPythonSuite();
            suite.Setup();

            PythonFixtureBuilder builder = new PythonFixtureBuilder(suite.Engine);
            List<PythonFixture> fixtures = builder.BuildFixtures();

            Assert.AreEqual(1, fixtures.Count);
            Assert.AreEqual(2, fixtures[0].Count);
        }

        [Test]
        public void RunPassingTest()
        {
            MyPythonSuite suite = new MyPythonSuite();
            suite.Setup();

            PythonFixtureBuilder builder = new PythonFixtureBuilder(suite.Engine);
            PythonFixture fixture = builder.BuildFixtures()[0];

            fixture["testPass"].Execute();
        }

        [Test]
        [ExpectedException(typeof (AssertionException), "this will fail")]
        public void RunFailingTest()
        {
            MyPythonSuite suite = new MyPythonSuite();
            suite.Setup();

            PythonFixtureBuilder builder = new PythonFixtureBuilder(suite.Engine);
            PythonFixture fixture = builder.BuildFixtures()[0];

            fixture["testFail"].Execute();
        }

        [Test]
        public void RunSetupAndTeardowns()
        {
            MyPythonSuite suite = new MyPythonSuite();
            suite.Setup();

            PythonFixtureBuilder builder = new PythonFixtureBuilder(suite.Engine);
            PythonFixture fixture = builder.BuildFixtures()[0];
            fixture.FixtureSetup();
            fixture.Setup();
            fixture.Teardown();
            fixture.FixtureTeardown();
        }
    }
}