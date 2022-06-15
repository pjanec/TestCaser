﻿using System;
using System.Reflection;
using System.Text;
using Scriban;
using Scriban.Runtime;
using System.Text.RegularExpressions;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TestCaser.Models.Results
{
    /// <summary>
	/// Data model for the TestResults report.
	/// Contains the results of all the test cases.
	/// </summary>
	public class ResultsModel
    {
		public List<TestCase> TestCases = new List<TestCase>();
    }

	public class TestCase
	{
		public string Name = string.Empty;
		public List<Dictionary<string,object>> Results = new List<Dictionary<string,object>>();	// from result json
		public bool Failed => Results.Any( (x) => (string)x["Status"] == EStatus.ERROR.ToString() ||  (string)x["Status"] == EStatus.FAIL.ToString() );
	}

	/// <summary>
	/// Gathers the results of all the test cases from the result files
	/// Generated by the commands.
	/// </summary>
	public class ModelLoader
	{
        static readonly Logger log = LogManager.GetCurrentClassLogger();

		public ResultsModel Load()
		{
			var m = new ResultsModel();

			// check if there is any failure or error reported in the results

			var dirInfo = new DirectoryInfo(Context.ResultFolder);
			var enumOpts = new EnumerationOptions()
			{
				MatchType = MatchType.Win32,
				RecurseSubdirectories = false,
				ReturnSpecialDirectories = false
			};

			// sort from oldest to youngest
			var files = dirInfo.GetFiles( "*.txt", enumOpts).OrderBy(x => x.CreationTime);

			foreach( var f in files )
			{
				var tc = new TestCase() { Name = Path.GetFileNameWithoutExtension( f.Name ) };
				m.TestCases.Add( tc );

				var lines = File.ReadAllLines( f.FullName );
				foreach( var line in lines )
				{
					try
					{
						var result = JsonConvert.DeserializeObject<Dictionary<string, object>>( line );

						tc.Results.Add( result );
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
