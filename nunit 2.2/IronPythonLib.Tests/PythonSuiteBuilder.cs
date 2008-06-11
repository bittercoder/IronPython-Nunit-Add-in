using System;
using IronPythonLib.Model;
using NUnit.Core;

namespace IronPythonLib.NUnit
{
    [SuiteBuilder]
    public class PythonSuiteExtensionBuilder : ISuiteBuilder
    {
        public TestSuite BuildFrom(Type type, int assemblyKey)
        {
            if (CanBuildFrom(type)) return new PythonSuiteExtension(type, assemblyKey);
            return null;
        }

        public bool CanBuildFrom(Type type)
        {
            return (typeof (AbstractPythonSuite).IsAssignableFrom(type) && (type != typeof (AbstractPythonSuite)));
        }
    }
}