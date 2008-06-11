using System;
using System.Collections.Generic;
using IronPython.Runtime.Types;
using IronPythonLib.Tests;
using NUnit.Framework;

namespace IronPythonLib.Model.Tests
{
    [TestFixture]
    public class AbstractPythonSuiteFixture
    {
        [Test]
        public void Create()
        {
            new MyPythonSuite();
        }

        [Test]
        public void Setup()
        {
            MyPythonSuite suite = new MyPythonSuite();
            suite.Setup();
        }

        [Test]
        public void RunCase()
        {
            // using a suite, make sure we can construct & run a method on the fixture in
            // the same fashion as we plan to in our implementation of the PythonFixtureBuilder.

            MyPythonSuite suite = new MyPythonSuite();
            suite.Setup();

            Object obj = suite.Engine.CreateMethod<Func<object>>("return MyPythonFixture()")();

            Assert.IsNotNull(obj);
            Assert.IsAssignableFrom(typeof (OldInstance), obj);

            Proc<object> proc =
                suite.Engine.CreateMethod<Proc<object>>("instance.testPass()",
                                                        new List<string>(new string[] {"instance"}));

            Proc proc2 = delegate
                {
                    proc(obj);
                };

            proc2();
        }

        [Test]
        public void RunInheritedCase()
        {
            // like RunCase() test, except we want to make sure we can execute inherited members

            InheritSuite suite = new InheritSuite();
            suite.Setup();

            Object obj = suite.Engine.CreateMethod<Func<object>>("return DerivedFixture()")();

            Assert.IsNotNull(obj);
            Assert.IsAssignableFrom(typeof (OldInstance), obj);

            Proc<object> proc =
                suite.Engine.CreateMethod<Proc<object>>("instance.testInBase()",
                                                        new List<string>(new string[] {"instance"}));

            Proc proc2 = delegate
                {
                    proc(obj);
                };

            proc2();
        }
    }
}