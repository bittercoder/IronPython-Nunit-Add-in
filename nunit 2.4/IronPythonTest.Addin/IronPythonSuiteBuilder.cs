using System;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace IronPythonTest.Addin
{
    public class IronPythonSuiteBuilder : ISuiteBuilder
    {
        #region ISuiteBuilder Members

        public Test BuildFrom(Type type)
        {
            if (CanBuildFrom(type))
                return new IronPythonSuiteExtension(type);

            return null;
        }

        public bool CanBuildFrom(Type type)
        {
            return Reflect.HasAttribute(type, "IronPythonTest.Framework.PythonFixtureAttribute", false);
        }

        #endregion
    }
}