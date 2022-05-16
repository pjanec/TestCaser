using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TestCaser
{
	/// <summary>
	/// Result class keeps the outcome of the commaend execution.
	/// It is serialized to the result record and also provided as data model to the HTML renderer.
	/// </summary>
	public class BaseResult
	{
		[JsonIgnore]
		public string CmdCode;

		//[JsonIgnore]
		//public DateTime TimeStamp;

		[JsonIgnore]
		public EStatus Status;

		public string Error;
	}
}
