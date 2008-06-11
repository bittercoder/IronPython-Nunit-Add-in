using System;
using System.IO;
using System.Reflection;
using IronPython.Hosting;
using NUnit.Framework;

namespace IronPythonLib.Model
{
    /// <summary>
    /// Python suites should be created from this suite.
    /// </summary>
    public abstract class AbstractPythonSuite
    {
        private PythonEngine _engine;
        private string[] _scripts;

        public AbstractPythonSuite(params string[] scripts)
        {
            if ((scripts == null) || (scripts.Length <= 0))
            {
                throw new ArgumentNullException("scripts");
            }

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
            PythonEngine engine = new PythonEngine();
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
            ExecuteScript(engine, typeof (AbstractPythonSuite).Assembly, "IronPythonLib.TestingSupport.py");
        }

        protected virtual void ExecuteScripts(PythonEngine engine)
        {
            foreach (string script in _scripts)
            {
                ExecuteScript(engine, GetType().Assembly, script);
            }
        }

        protected virtual void ExecuteScript(PythonEngine engine, Assembly assembly, string script)
        {
            Stream stream = assembly.GetManifestResourceStream(script);

            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream(GetType().Namespace + "." + script);
            }

            if (stream == null)
            {
                throw new ApplicationException(
                    string.Format("Could not find script: {0} in the assembly resources", script));
            }

            using (stream)
            using (StreamReader reader = new StreamReader(stream))
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