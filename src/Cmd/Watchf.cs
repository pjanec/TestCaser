using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestCaser.Cmd
{

	/// <summary>
	/// Starts monitoring a file for appending new lines from the current length of the file;
	/// The watcherId is then used in the regexf command to reference the file where to search for regexps.
	/// Note: regexf can create anonymous watcher but if so, then the regex is applied from the beginning of file
	/// If we don't want to search from the very beginning, we have to use the Watchf before the regexf
	/// to put the search location to the end of the file.
	/// </summary>
	public class Watchf : BaseCmd
	{
		public string FileSpec;
		public string Id;
		public bool FromBeginning;

		public override void ParseCmd( string[] cmd )
		{
			FileSpec = cmd[1];
			if( cmd.Length > 2 && Tools.IsJsonObj( cmd[2] ) ) JsonConvert.PopulateObject( cmd[2], this );
		}

		public override string Brief => Id;

		public override ExitCode Execute()
		{
			var fspec = TestCaser.FileSpec.From( FileSpec );
			var wf = new FileWatcher( Id, fspec.GetPath(), FileWatcher.Mode.Create );
			if( !FromBeginning )
			{
				wf.MoveToEnd();
			}
			wf.Save(); // remember new offset
			return ExitCode.Success;
		}
	}
}
