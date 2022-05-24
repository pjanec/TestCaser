using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestCaser
{
	/// <summary>
	/// Result class keeps the outcome of the command execution.
	/// It is serialized to the result record and also provided as data model to the HTML renderer.
	/// </summary>
	public class BaseResult
	{
		public DateTime TimeStamp;

		public string Phase;

		public string CmdCode;

		public JToken CmdData;

		[JsonConverter(typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
		public EStatus Status;

		/// <summary> brief info about the command arguments, shown in report, helping the user to
		///  identify what command the results are provided for  </summary>
		public string Brief;


		/// <summary> error description string if case of command failure </summary>
		public string Error;

		public string StackTrace; // FIXME: maybe different data type for stack trace

		public virtual string Details => null; // default text for result report (if no specialized rendering is used)
	}
}
