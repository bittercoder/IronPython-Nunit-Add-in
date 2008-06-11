using System.Collections;
using System.Collections.Generic;

namespace IronPythonLib.Model
{
    /// <summary>
    /// Represents a python fixture in our model.
    /// </summary>
    public class PythonFixture : IEnumerable<PythonTestCase>
    {
        private List<PythonTestCase> _cases = new List<PythonTestCase>();
        private string _name;
        private Proc _setup;
        private Proc _teardown;
        private Proc _fixtureSetup;
        private Proc _fixtureTeardown;

        public PythonFixture(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public void Add(PythonTestCase testCase)
        {
            _cases.Add(testCase);
        }

        public PythonTestCase this[int index]
        {
            get { return _cases[index]; }
        }

        public PythonTestCase this[string methodName]
        {
            get
            {
                return _cases.Find(delegate(PythonTestCase testCase)
                    {
                        return (string.Compare(methodName, testCase.Name, true) == 0);
                    });
            }
        }

        public void SetSetup(Proc proc)
        {
            _setup = proc;
        }

        public void Setup()
        {
            if (_setup != null) _setup();
        }

        public void SetTeardown(Proc proc)
        {
            _teardown = proc;
        }

        public void Teardown()
        {
            if (_teardown != null) _teardown();
        }

        public void SetFixtureSetup(Proc proc)
        {
            _fixtureSetup = proc;
        }

        public void FixtureSetup()
        {
            if (_fixtureSetup != null) _fixtureSetup();
        }

        public void SetFixtureTeardown(Proc proc)
        {
            _fixtureTeardown = proc;
        }

        public void FixtureTeardown()
        {
            if (_fixtureTeardown != null) _fixtureTeardown();
        }

        public int Count
        {
            get { return _cases.Count; }
        }

        #region IEnumerable<PythonTestCase> Members

        public IEnumerator<PythonTestCase> GetEnumerator()
        {
            return _cases.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _cases).GetEnumerator();
        }

        #endregion
    }
}