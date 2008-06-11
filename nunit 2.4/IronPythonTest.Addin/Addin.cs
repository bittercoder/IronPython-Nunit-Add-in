// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org/?p=license&r=2.4
// ****************************************************************

using NUnit.Core.Extensibility;

namespace IronPythonTest.Addin
{
    /// <summary>
    /// Summary description for Addin.
    /// </summary>
    [NUnitAddin(Name = "IronPythonSuiteExtension",
        Description = "Recognizes Test Fixtures Annotated with IronPythonFixture")]
    public class Addin : IAddin
    {
        #region IAddin Members

        public bool Install(IExtensionHost host)
        {
            IExtensionPoint builders = host.GetExtensionPoint("SuiteBuilders");

            if (builders == null)
                return false;

            builders.Install(new IronPythonSuiteBuilder());


            return true;
        }

        #endregion
    }
}