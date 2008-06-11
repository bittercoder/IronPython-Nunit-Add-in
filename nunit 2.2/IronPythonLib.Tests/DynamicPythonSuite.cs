using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using IronPythonLib.Model;

namespace IronPythonLib.Tests
{
    public class DynamicPythonSuite : AbstractPythonSuite
    {
        public DynamicPythonSuite()
            : base(FindSuitablePythonScripts())
        {
        }

        private static string[] FindSuitablePythonScripts()
        {
            List<string> scripts = new List<string>();

            Assembly assembly = typeof (DynamicPythonSuite).Assembly;

            string indication = "#test";

            foreach (string potentialScript in assembly.GetManifestResourceNames())
            {
                using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(potentialScript)))
                {
                    if (reader.ReadLine().Trim().StartsWith(indication, true, CultureInfo.InvariantCulture))
                    {
                        scripts.Add(potentialScript);
                    }
                }
            }

            return scripts.ToArray();
        }
    }
}