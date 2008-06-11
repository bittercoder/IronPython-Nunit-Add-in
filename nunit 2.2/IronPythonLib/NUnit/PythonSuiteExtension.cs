using System;
using System.Collections.Generic;
using IronPythonLib.Model;
using NUnit.Core;

namespace IronPythonLib.NUnit
{
    /// <summary>
    /// An NUnit Suite which wraps up a class derived from AbstractPythonSuite in our model.
    /// </summary>
    public class PythonSuiteExtension : TestSuite
    {
        public PythonSuiteExtension(Type fixtureType, int assemblyKey)
            : base(fixtureType, assemblyKey)
        {
            Fixture = Reflect.Construct(fixtureType);

            AbstractPythonSuite suite = Fixture as AbstractPythonSuite;
            suite.Setup();

            if (suite == null)
            {
                throw new ApplicationException("Fixture is not supported, must be derived from AbstractPythonSuite");
            }

            PythonFixtureBuilder builder = new PythonFixtureBuilder(suite.Engine);

            List<PythonFixture> fixtures = builder.BuildFixtures();

            foreach (PythonFixture fixture in fixtures)
            {
                PythonFixtureExtension wrappedFixure = new PythonFixtureExtension(fixture, assemblyKey);
                Add(wrappedFixure);
            }
        }
    }
}