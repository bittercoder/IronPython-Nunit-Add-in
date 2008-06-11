using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Core;

namespace IronPythonTest.Addin
{
    internal class IronPythonSuiteExtension : TestSuite
    {
        public IronPythonSuiteExtension(Type fixtureType)
            : base(fixtureType)
        {
            var suite = new PythonSuite(fixtureType, GetScripts(fixtureType));

            suite.Setup();

            Fixture = suite;

            var builder = new PythonFixtureBuilder(suite.Engine);

            foreach (PythonFixture fixture in builder.BuildFixtures())
            {
                Add(new PythonFixtureExtension(fixture));
            }
        }

        private string[] GetScripts(Type fixtureType)
        {
            var scripts = new List<string>();

            Attribute fixtureAttribute = Reflect.GetAttribute(fixtureType,
                                                              "IronPythonTest.Framework.PythonFixtureAttribute",
                                                              true);
            Attribute[] scriptAttributes = Reflect.GetAttributes(fixtureType,
                                                                 "IronPythonTest.Framework.ScriptAttribute",
                                                                 true);

            if (scriptAttributes != null)
                scripts.AddRange(scriptAttributes.Select(attr => (string) Reflect.GetPropertyValue(attr, "FileName")));

            var discoverEmbeddedResources =
                (bool) Reflect.GetPropertyValue(fixtureAttribute, "DiscoverEmbeddedResources");

            if (discoverEmbeddedResources)
            {
                var discoveryKey = (string) Reflect.GetPropertyValue(fixtureAttribute, "DiscoveryKey");

                scripts.AddRange(FindSuitablePythonScripts(fixtureType.Assembly, discoveryKey));
            }

            return scripts.ToArray();
        }

        private string[] FindSuitablePythonScripts(Assembly assembly, string discoveryKey)
        {
            var scripts = new List<string>();

            foreach (string potentialScript in assembly.GetManifestResourceNames())
            {
                using (var reader = new StreamReader(assembly.GetManifestResourceStream(potentialScript)))
                {
                    if (reader.ReadLine().Trim().StartsWith(discoveryKey, true, CultureInfo.InvariantCulture))
                    {
                        scripts.Add(potentialScript);
                    }
                }
            }

            return scripts.ToArray();
        }
    }
}