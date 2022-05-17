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

		public string FileId;
		public string RegexId;
		public bool NotMatch;

		public override string Brief => $"{FileId} {RegexId}";

		public class Result : BaseResult
		{					
			public RegexMatch Match;
		}

		public override void ParseCmd( string[] cmd )
		{
			FileId = cmd[1];
			RegexId = cmd[2];
			if( cmd.Length > 3 && Tools.IsJsonObj( cmd[3] ) ) JsonConvert.PopulateObject( cmd[3], this );
		}


		public override ExitCode Execute()
		{
			var fspec = FileSpec.From( FileId );

			// get lines from the watched file
			var wf = FileWatcher.Load( fspec.Watcher, fspec.GetPath() );
			var lines = wf.GetLines();
			wf.Save(); // remember new offset

			// apply regex
			try
			{
				var args = new FileRegEx.Args()
				{
					NotMatch = NotMatch
				};
				
				var re = new FileRegEx( RegexId, args );
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
