using System;
using System.Reflection;
using IronPythonLib.Model;
using NUnit.Core;

namespace IronPythonLib.NUnit
{
    /// <summary>
    /// A NUnit Fixture which wraps a "PythonFixture" in our corresponding model.
    /// </summary>
    public class PythonFixtureExtension : TestSuite
    {
        private PythonFixture _fixture;

        public PythonFixtureExtension(PythonFixture fixture, int assemblyKey)
            : base(fixture.Name, assemblyKey)
        {
            _fixture = fixture;

            testFramework = TestFramework.FromAssembly(GetType().Assembly);

            foreach (PythonTestCase testCase in _fixture)
            {
                Add(new PythonTestMethod(fixture.Name, testCase.Name, fixture, testCase));
            }
        }

        public override bool IsFixture
        {
            get { return true; }
        }

        public override void DoFixtureSetUp(TestResult suiteResult)
        {
            try
            {
                _fixture.FixtureSetup();

                Status = SetUpState.SetUpComplete;
            }
            catch (Exception ex)
            {
                if (ex is NunitException || ex is TargetInvocationException)
                    ex = ex.InnerException;

                if (testFramework.IsIgnoreException(ex))
                {
                    ShouldRun = false;
                    suiteResult.NotRun(ex.Message);
                    suiteResult.StackTrace = ex.StackTrace;
                    IgnoreReason = ex.Message;
                }
                else
                {
                    suiteResult.Failure(ex.Message, ex.StackTrace);
                    Status = SetUpState.SetUpFailed;
                }
            }
            finally
            {
                if (testFramework != null)
                    suiteResult.AssertCount = testFramework.GetAssertCount();
            }
        }

        public override void DoFixtureTearDown(TestResult suiteResult)
        {
            if (ShouldRun)
            {
                try
                {
                    Status = SetUpState.SetUpNeeded;
                    _fixture.FixtureTeardown();
                }
                catch (Exception ex)
                {
                    NunitException nex = ex as NunitException;
                    if (nex != null)
                        ex = nex.InnerException;

                    suiteResult.Failure(ex.Message, ex.StackTrace);
                }
                finally
                {
                    if (testFramework != null)
                        suiteResult.AssertCount += testFramework.GetAssertCount();
                }
            }
        }
    }
}