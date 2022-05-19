using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TestCaser
{
	/// <summary>
	/// Result class keeps the outcome of the command execution.
	/// It is serialized to the result record and also provided as data model to the HTML renderer.
	/// </summary>
	public class BaseResult
	{
		[JsonIgnore]
		public string CmdCode;

		/// <summary> brief info about the command arguments, shown in report, helping the user to
		///  identify what command the results are provided for  </summary>
		public string Brief;

		//[JsonIgnore]
		//public DateTime TimeStamp;

		[JsonIgnore]
		public EStatus Status;

		/// <summary> error description string if case of command failure </summary>
		public string Error;

		public string StackTrace; // FIXME: maybe different data type for stack trace
	}
}
