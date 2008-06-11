using System;
using System.Text;
using IronPythonLib.Model;
using NUnit.Core;

namespace IronPythonLib.NUnit
{
    /// <summary>
    /// An NUnit test case, which wraps up a PythonTestCase from our model.
    /// </summary>
    public class PythonTestMethod : TestCase
    {
        private PythonFixture _fixture;
        private PythonTestCase _testCase;

        public PythonTestMethod(string path, string name, PythonFixture fixture, PythonTestCase testCase)
            : base(path, name)
        {
            if (fixture == null) throw new ArgumentNullException("fixture");
            if (testCase == null) throw new ArgumentNullException("testCase");

            _fixture = fixture;
            _testCase = testCase;

            testFramework = TestFramework.FromAssembly(GetType().Assembly);
        }

        public override void Run(TestCaseResult testResult)
        {
            if (ShouldRun)
            {
                bool needParentTearDown = false;

                try
                {
                    if (Parent != null)
                    {
                        if (Parent.SetUpNeeded)
                        {
                            Parent.DoOneTimeSetUp(testResult);
                            needParentTearDown = Parent.SetUpComplete;
                        }

                        if (Parent.SetUpFailed)
                            testResult.Failure("TestFixtureSetUp Failed", testResult.StackTrace);
                    }

                    if (!testResult.IsFailure)
                        doRun(testResult);
                }
                catch (Exception ex)
                {
                    if (ex is NunitException)
                        ex = ex.InnerException;

                    RecordException(ex, testResult);
                }
                finally
                {
                    if (needParentTearDown)
                        Parent.DoOneTimeTearDown(testResult);
                }
            }
            else
            {
                testResult.NotRun(IgnoreReason);
            }
        }

        public virtual void doRun(TestCaseResult testResult)
        {
            DateTime start = DateTime.Now;

            try
            {
                _fixture.Setup();

                doTestCase(testResult);
            }
            catch (Exception ex)
            {
                if (ex is NunitException)
                    ex = ex.InnerException;

                RecordException(ex, testResult);
            }
            finally
            {
                doTearDown(testResult);

                DateTime stop = DateTime.Now;
                TimeSpan span = stop.Subtract(start);
                testResult.Time = (double) span.Ticks/(double) TimeSpan.TicksPerSecond;
            }
        }

        #region Invoke Methods by Reflection, Recording Errors

        private void doTearDown(TestCaseResult testResult)
        {
            try
            {
                _fixture.Teardown();
            }
            catch (Exception ex)
            {
                if (ex is NunitException)
                    ex = ex.InnerException;

                RecordTearDownException(ex, testResult);
            }
        }

        private void doTestCase(TestCaseResult testResult)
        {
            try
            {
                RunTestMethod(testResult);
                testResult.Success();
            }
            catch (Exception ex)
            {
                if (ex is NunitException)
                    ex = ex.InnerException;

                if (testFramework.IsIgnoreException(ex))
                    testResult.NotRun(ex.Message, BuildStackTrace(ex));
                else
                    RecordException(ex, testResult);
            }
        }

        public virtual void RunTestMethod(TestCaseResult testResult)
        {
            _testCase.Execute();
            testResult.Success();
        }

        #endregion

        #region Record Info About An Exception

        protected void RecordException(Exception ex, TestResult testResult)
        {
            if (testFramework.IsIgnoreException(ex))
                testResult.NotRun(ex.Message);
            else if (testFramework.IsAssertException(ex))
                testResult.Failure(ex.Message, ex.StackTrace);
            else
                testResult.Failure(BuildMessage(ex), BuildStackTrace(ex));
        }

        protected void RecordTearDownException(Exception exception, TestCaseResult testResult)
        {
            StringBuilder msg = new StringBuilder();
            StringBuilder st = new StringBuilder();

            msg.Append(testResult.Message);
            msg.Append(Environment.NewLine);
            msg.Append("TearDown : ");
            st.Append(testResult.StackTrace);
            st.Append(Environment.NewLine);
            st.Append("--TearDown");
            st.Append(Environment.NewLine);

            msg.Append(BuildMessage(exception));
            st.Append(BuildStackTrace(exception));
            testResult.Failure(msg.ToString(), st.ToString());
        }

        private string BuildMessage(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            if (testFramework.IsAssertException(exception))
                sb.Append(exception.Message);
            else
                sb.AppendFormat("{0} : {1}", exception.GetType().ToString(), exception.Message);

            Exception inner = exception.InnerException;
            while (inner != null)
            {
                sb.Append(Environment.NewLine);
                sb.AppendFormat("  ----> {0} : {1}", inner.GetType().ToString(), inner.Message);
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        private string BuildStackTrace(Exception exception)
        {
            if (exception.InnerException != null)
                return GetStackTrace(exception) + Environment.NewLine +
                       "--" + exception.GetType().Name + Environment.NewLine +
                       BuildStackTrace(exception.InnerException);
            else
                return GetStackTrace(exception);
        }

        private string GetStackTrace(Exception exception)
        {
            try
            {
                return exception.StackTrace;
            }
            catch (Exception)
            {
                return "No stack trace available";
            }
        }

        #endregion
    }
}