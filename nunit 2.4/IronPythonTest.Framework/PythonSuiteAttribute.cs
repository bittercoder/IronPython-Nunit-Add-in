using System;

namespace IronPythonTest.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PythonSuiteAttribute : Attribute
    {
        public PythonSuiteAttribute()
        {
            DiscoveryKey = "#test";
        }

        public bool DiscoverEmbeddedResources { get; set; }
        public string DiscoveryKey { get; set; }
    }
}