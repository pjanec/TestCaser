using System;
using System.Reflection;
using System.Text;
using Scriban;
using Scriban.Runtime;
using System.Text.RegularExpressions;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCaser.Models.Results
{
    public class ResultsModel
    {
		public List<TestCase> TestCases = new List<TestCase>();
    }

	public class TestCase
	{
		public string Name = string.Empty;
		public List<Record> Records = new List<Record>();
		public bool Failed => Records.Any( (x) => x.Failed );
	}

	public class Record
	{
		public string Status = string.Empty;
		public string Phase = string.Empty;
		public string Operation = string.Empty;
		public string[] Details = new string[0]; // type-specific
		public bool Failed => Status == "FAIL" || Status == "ERROR";
	}


	public class ModelLoader
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		public ResultsModel Load()
		{
			var m = new ResultsModel();

			// check if there is any failure or error reported in the results
			var resultFiles = Directory.GetFiles( Context.ResultFolder, "*.txt" );
			foreach( var fname in resultFiles )
			{
				var tc = new TestCase() { Name = Path.GetFileNameWithoutExtension( fname ) };
				m.TestCases.Add( tc );

				var lines = File.ReadAllLines( fname );
				foreach( var line in lines )
				{
					try
					{
						Result.ParseLine( line, out var rl );

						var chk = new Record()
						{
							Status = rl.Status,
							Operation = rl.Operation,
							Phase = rl.Phase,
							Details = rl.Details
						};

						tc.Records.Add( chk );
					}
					catch
					{
					}
				}
			}
			return m;
		}
	}
}
