using System;
using System.Collections.Generic;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Types;

namespace IronPythonTest.Addin
{
    /// <summary>
    /// This class encapsulates the functionality to build fixtures from all the fixtures
    /// which currently exist in a Python Engine.
    /// </summary>
    public class PythonFixtureBuilder
    {
        private const string FixtureBaseClass = "NUnitFixture";
        private const string TestMethodPrefix = "test";
        private readonly Func<OldClass, OldInstance> _createInstance;
        private readonly PythonEngine _engine;
        private readonly Func<OldClass, string, List> _findTestMethods;
        private readonly Func<OldClass, bool> _isNUnitFixture;

        public PythonFixtureBuilder(PythonEngine engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");
            _engine = engine;

            _findTestMethods =
                _engine.CreateMethod<Func<OldClass, string, List>>(@"return getTestCaseNames(testCaseClass, prefix)",
                                                                   Params("testCaseClass", "prefix"));
            _isNUnitFixture =
                _engine.CreateMethod<Func<OldClass, bool>>(
                    string.Format("return (obj != {0}) and (issubclass(obj, {0}))", FixtureBaseClass), Params("obj"));

            _createInstance = _engine.CreateMethod<Func<OldClass, OldInstance>>("return toCreate()", Params("toCreate"));
        }

        public List<PythonFixture> BuildFixtures()
        {
            var fixtures = new List<PythonFixture>();

            List<OldClass> pythonClasses = FindPythonClasses();

            foreach (OldClass pythonClass in pythonClasses)
            {
                fixtures.Add(BuildFixture(pythonClass));
            }

            return fixtures;
        }

        #region support methods

        private PythonFixture BuildFixture(OldClass pythonClass)
        {
            var fixture = new PythonFixture((string) pythonClass.__name__);

            OldInstance instance = _createInstance(pythonClass);

            // assing the test methods

            foreach (string methodName in FindTestMethods(pythonClass, TestMethodPrefix))
            {
                fixture.Add(new PythonTestCase(methodName, MakeProc(instance, methodName)));
            }

            // assign the setup and tear down methods

            fixture.SetFixtureSetup(MakeProc(instance, "setUpFixture"));
            fixture.SetFixtureTeardown(MakeProc(instance, "tearDownFixture"));
            fixture.SetSetup(MakeProc(instance, "setUp"));
            fixture.SetTeardown(MakeProc(instance, "tearDown"));

            // all done

            return fixture;
        }

        private Proc MakeProc(OldInstance instance, string methodName)
        {
            var nakedProc =
                _engine.CreateMethod<Proc<object>>(string.Format("instance.{0}()", methodName), Params("instance"));

            Proc testCase = () => nakedProc(instance);

            return testCase;
        }

        private static List<string> Params(params string[] parameters)
        {
            return new List<string>(parameters);
        }

        private List<string> FindTestMethods(OldClass obj, string prefix)
        {
            return _findTestMethods(obj, prefix).ToList<string>();
        }

        private List<OldClass> FindPythonClasses()
        {
            var pythonClasses = new List<OldClass>();

            foreach (object obj in _engine.Globals.Values)
            {
                var pythonClass = obj as OldClass;
                if (pythonClass != null)
                {
                    if (IsNUnitFixture(pythonClass))
                    {
                        pythonClasses.Add(pythonClass);
                    }
                }
            }

            return pythonClasses;
        }

        private bool IsNUnitFixture(OldClass obj)
        {
            return _isNUnitFixture(obj);
        }

        #endregion
    }
}