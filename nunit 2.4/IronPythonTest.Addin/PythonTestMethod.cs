using System;
using NUnit.Core;

namespace IronPythonTest.Addin
{
    /// <summary>
    /// An NUnit test case, which wraps up a PythonTestCase from our model.
    /// </summary>
    public class PythonTestMethod : TestCase
    {
        private readonly PythonFixture _fixture;
        private readonly PythonTestCase _testCase;

        public PythonTestMethod(string path, string name, PythonFixture fixture, PythonTestCase testCase)
            : base(path, name)
        {
            if (fixture == null) throw new ArgumentNullException("fixture");
            if (testCase == null) throw new ArgumentNullException("testCase");

            _fixture = fixture;
            _testCase = testCase;
        }


        public override void Run(TestCaseResult testResult)
        {
            DateTime start = DateTime.Now;

            try
            {
                try
                {
                    _fixture.Setup();
                }
                catch (Exception ex)
                {
                    testResult.Error(ex, FailureSite.SetUp);
                    return;
                }

                _testCase.Execute();
                testResult.Success();
            }
            catch (Exception ex)
            {
                testResult.Error(ex, FailureSite.Test);
            }
            finally
            {
                try
                {
                    _fixture.Teardown();
                }
                catch (Exception ex)
                {
                    testResult.Error(ex, FailureSite.TearDown);
                }

                DateTime stop = DateTime.Now;
                TimeSpan span = stop.Subtract(start);
                testResult.Time = span.Ticks/(double) TimeSpan.TicksPerSecond;
            }
        }
    }
}