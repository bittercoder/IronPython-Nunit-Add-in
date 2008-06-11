using System;

namespace IronPythonTest.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScriptAttribute : Attribute
    {
        public string FileName { get; set; }
    }
}