using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Searches a file for given regular expression, either from beginning or
	/// just the lines appended since the last query.
	/// </summary>
	public class Regexf : BaseCmd
	{
		[JsonIgnore]		
		public override string Code => "regexf";

		public string FileSpec;
		public string RegexSpec;
		public bool NotMatch;
		public bool FromBeginning;

		public override string Brief => $"{FileSpec} {RegexSpec}";

		public class Result : BaseResult
		{					
			public RegexMatch Match;
		}

		public override void ParseCmd( string[] cmd )
		{
			FileSpec = cmd[1];
			RegexSpec = cmd[2];
			if( cmd.Length > 3 && Tools.IsJsonObj( cmd[3] ) ) JsonConvert.PopulateObject( cmd[3], this );
		}


		public override ExitCode Execute()
		{
			var fspec = TestCaser.FileSpec.From( FileSpec );

			// get lines from the watched file
			var wf = new FileWatcher( fspec.Watch, fspec.GetPath(), FileWatcher.Mode.LoadOrCreate );
			var lines = wf.GetLines();
			wf.Save(); // remember new offset

			// apply regex
			try
			{
				var args = new FileRegexSearcher.Args()
				{
					NotMatch = NotMatch
				};
				
				var regexSpec = TestCaser.RegexSpec.From(RegexSpec );
				var re = new FileRegexSearcher( regexSpec.GetRegex(), args );
				bool success = re.Search( lines );
				if( !success )
				{	
					// log the result
					Results.Add( new Result() { Brief=Brief, CmdCode=Code, Status=EStatus.ERROR, Error = "No match" });
					return ExitCode.Failure;
				}
			}
			catch(Exception ex)
			{
				Results.Add( new Result() { Brief=Brief, CmdCode = Code, Status = EStatus.ERROR, Error = ex.Message } );

				return ExitCode.Failure;
			}

			return ExitCode.Success;
		}
	}
}
