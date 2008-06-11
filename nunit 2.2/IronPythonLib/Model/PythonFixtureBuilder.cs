using System;
using System.Collections.Generic;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Types;

namespace IronPythonLib.Model
{
    /// <summary>
    /// This class encapsulates the functionality to build fixtures from all the fixtures
    /// which currently exist in a Python Engine.
    /// </summary>
    public class PythonFixtureBuilder
    {
        private const string FixtureBaseClass = "NUnitFixture";
        private const string TestMethodPrefix = "test";
        private Func<bool, OldClass> _isNUnitFixture;
        private Func<List, OldClass, string> _findTestMethods;
        private Func<OldInstance, OldClass> _createInstance;
        private PythonEngine _engine;

        public PythonFixtureBuilder(PythonEngine engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");
            _engine = engine;

            _findTestMethods =
                _engine.CreateMethod<Func<List, OldClass, string>>(@"return getTestCaseNames(testCaseClass, prefix)",
                                                                   Params("testCaseClass", "prefix"));
            _isNUnitFixture =
                _engine.CreateMethod<Func<bool, OldClass>>(
                    string.Format("return (obj != {0}) and (issubclass(obj, {0}))", FixtureBaseClass), Params("obj"));

            _createInstance = _engine.CreateMethod<Func<OldInstance, OldClass>>("return toCreate()", Params("toCreate"));
        }

        public List<PythonFixture> BuildFixtures()
        {
            List<PythonFixture> fixtures = new List<PythonFixture>();

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
            PythonFixture fixture = new PythonFixture((string) pythonClass.__name__);

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
            Proc<object> nakedProc =
                _engine.CreateMethod<Proc<object>>(string.Format("instance.{0}()", methodName), Params("instance"));

            Proc testCase = delegate
                {
                    nakedProc(instance);
                };

            return testCase;
        }

        private List<string> Params(params string[] parameters)
        {
            return new List<string>(parameters);
        }

        private List<string> FindTestMethods(OldClass obj, string prefix)
        {
            List list = _findTestMethods(obj, prefix);

            return new List<string>(new StrongEnumerator<string>(list));
        }

        private List<OldClass> FindPythonClasses()
        {
            List<OldClass> pythonClasses = new List<OldClass>();

            foreach (object obj in _engine.Globals.Values)
            {
                OldClass pythonClass = obj as OldClass;
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