using System;
using System.IO;
using System.Reflection;
using IronPython.Hosting;
using NUnit.Framework;

namespace IronPythonTest.Addin
{
    /// <summary>
    /// Python suites should be created from this suite.
    /// </summary>
    public class PythonSuite
    {
        private readonly Type _fixtureType;
        private readonly string[] _scripts;
        private PythonEngine _engine;

        public PythonSuite(Type fixtureType, params string[] scripts)
        {
            if (fixtureType == null) throw new ArgumentNullException("fixtureType");
            if ((scripts == null) || (scripts.Length <= 0)) throw new ArgumentNullException("scripts");

            _fixtureType = fixtureType;
            _scripts = scripts;
        }

        public string[] Scripts
        {
            get { return _scripts; }
        }

        public PythonEngine Engine
        {
            get { return _engine; }
        }

        public void Setup()
        {
            var engine = new PythonEngine();
            PrepareEngine(engine);
            ExecuteScripts(engine);

            _engine = engine;
        }

        protected virtual void PrepareEngine(PythonEngine engine)
        {
            engine.Import("clr");
            engine.Import("System");
            engine.LoadAssembly(typeof (TestAttribute).Assembly);
            engine.Execute("from NUnit.Framework import *");
            ExecuteScript(engine, typeof (PythonSuite).Assembly, "IronPythonTest.Addin.TestingSupport.py");
        }

        protected virtual void ExecuteScripts(PythonEngine engine)
        {
            foreach (string script in _scripts)
            {
                ExecuteScript(engine, _fixtureType.Assembly, script);
            }
        }

        private Stream GetStream(Assembly assembly, string name)
        {
            return (assembly.GetManifestResourceInfo(name) != null) ? assembly.GetManifestResourceStream(name) : null;
        }

        private Stream GetStream(Assembly assembly, Type type, string name)
        {
            return GetStream(assembly, type.Namespace + "." + name);
        }

        protected virtual void ExecuteScript(PythonEngine engine, Assembly assembly, string script)
        {
            Stream stream = GetStream(assembly, script) ?? GetStream(assembly, _fixtureType, script);

            if (stream == null)
            {
                throw new ApplicationException(
                    string.Format(
                        "Could not find script: {0} in the assembly resources, ensure the name is spelt correctly and that the C# fixture class and resource are in the same namespace.",
                        script));
            }

            using (stream)
            using (var reader = new StreamReader(stream))
            {
                try
                {
                    engine.Execute(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Failed to execute script: {0}", script), ex);
                }
            }
        }
    }
}