// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.IO;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Text.Method;
using Android.Widget;
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite.Runner;
using NUnitLite.Runner.Android;

namespace NUnit.Framework
{
	/// <summary>
	/// The base Activity from which to derive a test view
	/// </summary>
	[Activity (Label = "TestSuiteActivity")]			
	public class TestSuiteActivity : Activity
	{
		private readonly ITestAssemblyRunner _runner = new DefaultTestAssemblyRunner(new DefaultTestAssemblyBuilder());
		private TextWriter _writer;
	    private TextView _textView;

		/// <summary>
		/// Adds an assembly to be tested
		/// </summary>
		/// <param name="assembly">Assembly.</param>
		public void AddTest (Assembly assembly)
		{
			_runner.Load (assembly, new Dictionary<string, string> ());
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.TestSuiteActivity);

		    _textView = (TextView)FindViewById(Resource.Id.testOutput);
            _textView.MovementMethod = new ScrollingMovementMethod();
			_writer = new TextViewWriter (_textView);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

		    _textView.Text = string.Empty;

			TextUI.WriteHeader(_writer);
			TextUI.WriteRuntimeEnvironment(_writer);

			ThreadPool.QueueUserWorkItem (delegate
			{
				ExecuteTests ();
			});
		}

		private void ExecuteTests()
		{
			ITestResult result = _runner.Run (TestListener.NULL, TestFilter.Empty);
			var reporter = new ResultReporter (result, _writer);

			reporter.ReportResults ();

			//ResultSummary summary = reporter.Summary;

//			this.Total.Text = summary.TestCount.ToString();
//			this.Failures.Text = summary.FailureCount.ToString();
//			this.Errors.Text = summary.ErrorCount.ToString();
//			this.NotRun.Text = summary.NotRunCount.ToString();
//			this.Passed.Text = summary.PassCount.ToString();
//			this.Inconclusive.Text = summary.InconclusiveCount.ToString();
//
//			this.Notice.Visibility = Visibility.Collapsed;		
		}
	}
}

