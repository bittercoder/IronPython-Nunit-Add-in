using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using IronPython.Hosting;
using IronPython.Runtime.Calls;
using NUnit.Framework;

namespace IronPythonLib.Tests
{
    public delegate string FormatUserDetailsDelegate(string user, int age);

    [TestFixture]
    public class PostsOneAndTwoFixture
    {
        [Test]
        public void GetGoing()
        {
            PythonEngine engine = new PythonEngine();
            engine.Execute("myString = 'hello'");
            Assert.AreEqual("hello reader", engine.Evaluate("myString + ' reader'"));
        }


        [Test]
        public void StatementsToStronglyTypedDelegate()
        {
            PythonEngine engine = new PythonEngine();
            List<string> parameters = new List<string>(new string[] {"age"});
            Converter<int, string> converter =
                engine.CreateMethod<Converter<int, string>>("return str(age+1)", parameters);
            Assert.AreEqual("11", converter(10));
        }

        [Test]
        public void ExtendingClass()
        {
            PythonEngine engine = new PythonEngine();

            engine.Import("clr");
            engine.LoadAssembly(typeof (IStringTransformer).Assembly);
            engine.ExecuteToConsole("from IronPythonLib import *");

            engine.Execute(
                @"class MyTransformer(IStringTransformer):        
    def Transform(self, input):
        return input + "" is now transformed""");

            IStringTransformer transformer = engine.EvaluateAs<IStringTransformer>("MyTransformer()");
            Assert.AreEqual("input is now transformed", transformer.Transform("input"));
        }

        [Test]
        public void HookingToEvents()
        {
            PythonEngine engine = new PythonEngine();

            ManualResetEvent waitHandle = new ManualResetEvent(false);
            WebClient client = new WebClient();

            List<string> results = new List<string>();

            engine.Globals["client"] = client;
            engine.Globals["results"] = results;

            engine.Execute(
                @"def storeResult(sender, e):
    results.Add(e.Result)
    e.UserState.Set()

# assign out storeResult function as an event handler for the client class
client.DownloadStringCompleted += storeResult
");

            client.DownloadStringAsync(
                new Uri(
                    "http://api.feedburner.com/awareness/1.0/GetFeedData?uri=http://feeds.feedburner.com/BitterCoder"),
                waitHandle);

            Assert.IsTrue(waitHandle.WaitOne(10000, false), "timeout occured after 10 seconds");

            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(results[0].StartsWith("<?xml"));
        }

        [Test]
        [
            ExpectedException(typeof (ArgumentException),
                "T must be a concrete delegate, not MulticastDelegate or Delegate")]
        public void LooseDelegateAndDynamicInvoke()
        {
            PythonEngine engine = new PythonEngine();

            List<string> parameters = new List<string>(new string[] {"name", "age"});

            Delegate func =
                engine.CreateMethod<Delegate>("return ‘my name is’ + name + ‘ and my age is ‘ + str(age)", parameters);
            string result = (string) func.DynamicInvoke("alex", 26);

            Assert.AreEqual("my name is alex and my age is 25", result);
        }

        [Test]
        public void StronglyTypedDelegate()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context.Add("user", "alex");
            context.Add("age", 26);

            PythonEngine engine = new PythonEngine();

            FormatUserDetailsDelegate func =
                engine.CreateMethod<FormatUserDetailsDelegate>("return user + ' is ' + str(age) + ' years old'",
                                                               new List<string>(context.Keys));
            object result = func.DynamicInvoke(new List<object>(context.Values).ToArray());

            Assert.AreEqual("alex is 26 years old", result);
        }

        [Test]
        public void GeneratingPythonFunction()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context.Add("user", "alex");
            context.Add("age", 26);

            List<string> parameters = new List<string>(context.Keys);
            List<object> values = new List<object>(context.Values);

            PythonEngine engine = new PythonEngine();

            engine.Execute(
                GenerateFunction("func", parameters.ToArray(), "return user + ' is ' + str(age) + ' years old'"));

            PythonFunction func1 = engine.EvaluateAs<PythonFunction>("func");

            engine.Execute(
                GenerateFunction("func", parameters.ToArray(),
                                 "return user + ' is ' + str(age+1) + ' years old next year'"));

            PythonFunction func2 = engine.EvaluateAs<PythonFunction>("func");

            object result1 = func1.Call(values.ToArray());
            Assert.AreEqual("alex is 26 years old", result1);

            object result2 = func2.Call(values.ToArray());
            Assert.AreEqual("alex is 27 years old next year", result2);
        }

        [ThereBeDragons("Only use as a last resort")]
        public delegate object UntypedDelegate(params object[] parameters);

        [Test]
        public void UntypedDelegateForPythonFunction()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context.Add("user", "alex");
            context.Add("age", 26);

            List<string> parameters = new List<string>(context.Keys);
            List<object> values = new List<object>(context.Values);

            PythonEngine engine = new PythonEngine();

            engine.Execute(
                GenerateFunction("func", parameters.ToArray(), "return user + ' is ' + str(age) + ' years old'"));

            PythonFunction func1 = engine.EvaluateAs<PythonFunction>("func");

            UntypedDelegate func1Delegate = delegate(object[] param)

                {
                    return func1.Call(param);
                };

            object result1 = func1Delegate(values.ToArray());
            Assert.AreEqual("alex is 26 years old", result1);
        }


        private static string GenerateFunction(string functionName, string[] parameters, string statements)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("def {0}(", functionName);

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0) builder.AppendFormat(", ");
                builder.AppendFormat(parameters[i]);
            }

            builder.AppendFormat("):\r\n    ");

            builder.AppendFormat(statements.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n    "));

            return builder.ToString();
        }

        [Test]
        public void GenerateSimpleFunction()
        {
            Assert.AreEqual(@"def func(user, age):
    return user + ' is ' + str(age) + ' years old'",
                            GenerateFunction("func", new string[] {"user", "age"},
                                             "return user + ' is ' + str(age) + ' years old'"));
        }


        [Test]
        public void WithCustomDelegate()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context.Add("user", "alex");
            context.Add("age", 26);

            Proc testCore = delegate
                {
                    List<string> parameters = new List<string>(context.Keys);
                    List<object> values = new List<object>(context.Values);

                    PythonEngine engine = new PythonEngine();

                    Type[] parameterTypes = new Type[context.Count];
                    for (int i = 0; i < parameterTypes.Length; i++) parameterTypes[i] = typeof (object);

                    Type delegateType = CreateCustomDelegate(typeof (object), parameterTypes);

                    Type pythonEngine = typeof (PythonEngine);
                    MethodInfo genericMethod =
                        pythonEngine.GetMethod("CreateMethod", new Type[] {typeof (string), typeof (IList<string>)});
                    MethodInfo method = genericMethod.MakeGenericMethod(delegateType);

                    object result =
                        method.Invoke(engine,
                                      new object[] {"return user + ' is ' + str(age) + ' years old'", parameters});

                    Delegate myDelegate = (Delegate) result;
                    Assert.AreEqual("alex is 26 years old", myDelegate.DynamicInvoke(values.ToArray()));
                };

            // try it with our two keys in the dictionary

            testCore();

            // try it with another key in the dictionary

            context.Add("monkey_colour", "brown");
            testCore();
        }

        public Type CreateCustomDelegate(Type returnType, params Type[] parameterTypes)
        {
            AssemblyName assembly = new AssemblyName();
            assembly.Name = "CreateCustomDelegateAssembly";

            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("TempModule");

            TypeBuilder typeBuilder =
                moduleBuilder.DefineType("TempDelegateType",
                                         TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class |
                                         TypeAttributes.AnsiClass | TypeAttributes.AutoClass, typeof (MulticastDelegate));

            ConstructorBuilder constructorBuilder =
                typeBuilder.DefineConstructor(
                    MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                    CallingConventions.Standard, new Type[] {typeof (object), typeof (int)});
            constructorBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("Invoke",
                                         MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                                         MethodAttributes.Virtual, returnType, parameterTypes);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);

            return typeBuilder.CreateType();
        }
    }

    /// <summary>
    /// This marker attribute decorate dangerous parts of the code
    /// where for some reason the developer decided to do something in a way
    /// that is not straight-forward.
    /// This usually means that this is using some functionality that is not fully supported.
    /// </summary>
    public class ThereBeDragonsAttribute : Attribute
    {
        public ThereBeDragonsAttribute()
        {
        }

        public ThereBeDragonsAttribute(string whyAreYouUsingThis)
        {
        }
    }
}