using System;
using System.Reflection;
using NUnit.Core;

namespace IronPythonTest.Addin
{
    /// <summary>
    /// A NUnit Fixture which wraps a "PythonFixture" in our corresponding model.
    /// </summary>
    public class PythonFixtureExtension : TestSuite
    {
        public PythonFixtureExtension(PythonFixture fixture)
            : base(typeof (PythonFixture))
        {
            Fixture = fixture;
            SetFixtureName(fixture);
            SetFixtureSetupAndTearDownMethods(fixture);
            AddTestMethodsToFixture(fixture);
        }

        private void AddTestMethodsToFixture(PythonFixture fixture)
        {
            foreach (PythonTestCase testCase in fixture)
            {
                Add(new PythonTestMethod(fixture.Name, testCase.Name, fixture, testCase));
            }
        }

        private void SetFixtureSetupAndTearDownMethods(PythonFixture fixture)
        {
            Type fixtureType = fixture.GetType();

            fixtureSetUp = Reflect.GetNamedMethod(fixtureType, "FixtureSetup",
                                                  BindingFlags.Public | BindingFlags.Instance);
            fixtureTearDown = Reflect.GetNamedMethod(fixtureType, "FixtureTeardown",
                                                     BindingFlags.Public | BindingFlags.Instance);
        }

        private void SetFixtureName(PythonFixture fixture)
        {
            TestName.FullName = fixture.Name;
            TestName.Name = fixture.Name;
        }
    }
}